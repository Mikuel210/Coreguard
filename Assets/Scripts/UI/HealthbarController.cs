using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private bool hide;
    
    private Slider _slider;
    
    void Start()
    {
        health ??= transform.parent.GetComponent<Health>();
        _slider = transform.Find("Slider").GetComponent<Slider>();
    }

    void Update()
    {
        _slider.value = health.CurrentHealth / health.MaximumHealth;
        
        if (hide && health.CurrentHealth == health.MaximumHealth)
            _slider.gameObject.SetActive(false);
        else
            _slider.gameObject.SetActive(true);
    }
}
