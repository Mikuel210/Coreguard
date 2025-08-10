using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damageToCore;
    [SerializeField] private Slider healthbarSlider;
    
    private Health _health;
    private Animator _animator;    
    
    void Start()
    {
        _health = GetComponent<Health>();
        _animator = GetComponent<Animator>();
        
        _health.OnDeath += () =>
            Destroy(gameObject);
    }
    
    void Update()
    {
        UpdateMovement();
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Core"))
        {
            other.GetComponent<Health>().TakeDamage(damageToCore);
            Destroy(gameObject);
        }
    }
}
