using UnityEngine;

public class CoreController : MonoBehaviour
{
    [SerializeField] private float size;
    [SerializeField] private float sizeSmoothing;
    private float _currentSize;

    [SerializeField] private float growthOnClick;
    
    void Start()
    {
        _currentSize = size;
    }
    
    void FixedUpdate()
    {
        _currentSize += (size - _currentSize) * sizeSmoothing;
        transform.localScale = Vector3.one * _currentSize;
    }
    
    void OnMouseOver()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        
        GameManager.Instance.GainEnergy(1);
        _currentSize += growthOnClick;
    }
}
