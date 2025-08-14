using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using GameObject = UnityEngine.GameObject;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private int energyPerShot;
    [SerializeField] private float range;
    [SerializeField] private float fireRate;
    [SerializeField] private float shootTime;
    [SerializeField] private float lineWidth;

    private Transform _gunAnchor;
    private LineRenderer _lineRenderer;
    private AudioSource _audioSource;
    
    private float _time;
    private bool _shooting;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gunAnchor = transform.Find("GunAnchor");
        _lineRenderer = GetComponent<LineRenderer>();
        _audioSource = GetComponent<AudioSource>();

        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;
        
        GetComponent<Light2D>().pointLightOuterRadius = range;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;

        if (!_shooting)
        {
            if (_time < fireRate - shootTime) return;
            if (GameManager.Instance.Energy < energyPerShot) return;
            
            GameObject enemy = GetNearestEnemy();
            if (!enemy) return;

            _shooting = true;
            _time = 0;
            
            GameManager.Instance.LoseEnergy(energyPerShot);
            enemy.GetComponent<Health>().TakeDamage(damage);

            UpdateShooting(enemy);
            
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.Play();
            
            Utils.Popup(damage.ToString(), enemy.transform.position, new (209 / 255f, 6 / 255f, 0 / 255f));
        }
        else
        {
            UpdateShooting();
            
            if (_time < shootTime) return;

            _shooting = false;
            _time = 0;
            
            _lineRenderer.positionCount = 0;
        }
    }

    private GameObject GetNearestEnemy()
    {
        List<GameObject> enemiesInRange = GetEnemiesInRange();
        if (enemiesInRange.Count == 0) return null;
            
        GameObject enemy = enemiesInRange
            .OrderBy(e => (e.transform.position - transform.position).sqrMagnitude)
            .First();

        return enemy;
    }

    private void UpdateShooting(GameObject enemy = null)
    {
        if (!enemy)
            enemy = GetNearestEnemy();

        if (!enemy)
        {
            _lineRenderer.positionCount = 0;
            return;
        }
        
        Vector2 direction = (enemy.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(enemy.transform.position, transform.position);
            
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _gunAnchor.rotation = Quaternion.Euler(0f, 0f, angle);
        
        _lineRenderer.positionCount = 3;
        _lineRenderer.SetPosition(0, Vector3.zero);
        _lineRenderer.SetPosition(1, transform.position);
        _lineRenderer.SetPosition(2, transform.position + (Vector3)direction * distance);
    }
    
    private List<GameObject> GetEnemiesInRange()
    {
        List<GameObject> output = new();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
                output.Add(hitCollider.gameObject);
        }

        return output;
    }
}
