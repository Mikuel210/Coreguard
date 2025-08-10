using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [field: SerializeField] public float MaximumHealth { get; private set; } = 100f;

    [SerializeField] private float _currentHealth;

    public float CurrentHealth
    {
        get => _currentHealth;
        
        private set
        {
            _currentHealth = Mathf.Clamp(value, 0, MaximumHealth);
            
            healthChangedEventBus.Invoke();
            OnHealthChanged?.Invoke();

            if (_currentHealth <= 0)
            {
                deathEventBus.Invoke();
                OnDeath?.Invoke();
            }
        }
    }
    
    public EventBus deathEventBus;
    public EventBus healthChangedEventBus;
    
    public event Action OnDeath;
    public event Action OnHealthChanged;
    
    public void TakeDamage(float damage) => CurrentHealth -= damage;
    public void Heal(float heal) => CurrentHealth += heal;
    
    void Start() => CurrentHealth = MaximumHealth;
}
