using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float coreRepulsionImpulse;
    [SerializeField] private float coreRepulsionForce;

    private Rigidbody2D _rigidbody;
    private Animator _animator;    
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateMovement();   
    }

    void UpdateMovement()
    {
        // Get input
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        
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
        if (!other.CompareTag("Core")) return;
        
        // Apply repulsive force
        Vector2 direction = (transform.position - other.transform.position).normalized;
        _rigidbody.AddForce(direction * coreRepulsionImpulse, ForceMode2D.Impulse);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Core")) return;
        
        // Apply repulsive force
        Vector2 direction = (transform.position - other.transform.position).normalized;
        _rigidbody.AddForce(direction * coreRepulsionForce, ForceMode2D.Force);
    }
}
