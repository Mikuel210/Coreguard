using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damageToCore;
    [SerializeField] private float damageToBuildings;
    [SerializeField] private Slider healthbarSlider;
    
    private Health _health;
    private Animator _animator;

    private List<GameObject> _buildingsBeingTouched = new();
    
    void Start()
    {
        _health = GetComponent<Health>();
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        UpdateMovement();
        UpdateBuildingDamage();
    }

    void UpdateMovement()
    {
        // Get direction
        Vector2 direction = (Vector3.zero - transform.position).normalized;
        float x = Mathf.Clamp(direction.x, -1, 1);
        float y = Mathf.Clamp(direction.y, -1, 1);
        
        // Get movement vector
        Vector2 directionVector = new(x, y);
        directionVector.Normalize();
        directionVector *= speed;
        
        // Translate
        transform.Translate(directionVector * Time.deltaTime);
        
        // Update animations
        _animator.SetInteger("x", Mathf.RoundToInt(x));
        _animator.SetInteger("y", Mathf.RoundToInt(y));
    }
    
    private float _damageTime;
    void UpdateBuildingDamage()
    {
        _damageTime += Time.deltaTime;

        if (_damageTime < 1) return;

        bool error = true;
        
        for (int i = _buildingsBeingTouched.Count - 1; i >= 0; i--)
        {
            GameObject building = _buildingsBeingTouched[i];
            building.GetComponent<Health>().TakeDamage(damageToBuildings);
        }
        
        _damageTime = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Core")) return;
        
        other.GetComponent<Health>().TakeDamage(damageToCore);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.CompareTag("Building")) return;
        _buildingsBeingTouched.Add(other.gameObject);
    }
    
    void OnCollisionExit2D(Collision2D other) => _buildingsBeingTouched.Remove(other.gameObject);
}
