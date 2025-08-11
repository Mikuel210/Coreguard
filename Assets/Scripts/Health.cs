using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    [field: SerializeField] public float MaximumHealth { get; private set; } = 100f;

    [SerializeField] private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        
        private set
        {
            currentHealth = Mathf.Clamp(value, 0, MaximumHealth);
            
            healthChangedEventBus.Invoke();
            OnHealthChanged?.Invoke();

            if (currentHealth <= 0)
            {
                deathEventBus.Invoke();
                OnDeath?.Invoke();
            }
        }
    }
    
    [SerializeField] private bool destroyOnDeath;
    
    public EventBus deathEventBus;
    public EventBus healthChangedEventBus;
    
    public event Action OnDeath;
    public event Action OnHealthChanged;
    
    public void TakeDamage(float damage) => CurrentHealth -= damage;
    public void Heal(float heal) => CurrentHealth += heal;

    void Start()
    {
        CurrentHealth = MaximumHealth;
        
        if (destroyOnDeath)
            OnDeath += () => Destroy(gameObject);
    }
}
