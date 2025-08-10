using UnityEngine;

public class EnergyHarvester : MonoBehaviour
{
    [SerializeField] private float interval = 1f;
    [SerializeField] private int energy = 1;
    [SerializeField] private float width = 1;
    
    private LineRenderer _lineRenderer;
    private float _time;
    
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, Vector3.zero);
        _lineRenderer.SetPosition(1, transform.position);

        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth = width;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;

        if (_time < interval) return;

        GameManager.Instance.GainEnergy(energy);
        _time = 0;
    }
}
