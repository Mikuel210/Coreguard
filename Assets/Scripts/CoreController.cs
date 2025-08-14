using Helpers;
using UnityEngine;

public class CoreController : Singleton<CoreController>
{
    [SerializeField] private float size;
    [SerializeField] private float sizeSmoothing;
    private float _currentSize;

    [SerializeField] private float growthOnClick;
    
    private AudioSource _audioSource; 
    
    void Start()
    {
        _currentSize = size;
        _audioSource = GetComponent<AudioSource>();
    }
    
    void FixedUpdate()
    {
        _currentSize += (size - _currentSize) * sizeSmoothing;
        transform.localScale = Vector3.one * _currentSize;
    }
    
    void OnMouseOver()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (PauseSystem.Instance.IsPaused) return;
        
        GameManager.Instance.GainEnergy(1);
        _currentSize += growthOnClick;

        _audioSource.pitch = _currentSize - 4;
        _audioSource.Play();
        
        float x = Random.Range(-100, 101) / 100f;
        float y = Random.Range(-100, 101) / 100f;
        
        Vector3 position = transform.position + Vector3.up * 3.5f + new Vector3(x, y);
        Utils.Popup("1", position, new (14 / 255f, 5 / 255f, 249 / 255f), 0.015f);
    }
}
