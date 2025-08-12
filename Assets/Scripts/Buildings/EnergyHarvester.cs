using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergyHarvester : MonoBehaviour
{
    [SerializeField] private float interval = 1f;
    [SerializeField] private int energy = 1;
    [SerializeField] private float width = 1;
    [SerializeField] private bool connectToCore = true;
    
    private LineRenderer _lineRenderer;
    private float _time;
    
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        BuildingSystem.Instance.OnBuildingPlacedOrDestroyed += UpdateLineRenderer;
        
        UpdateLineRenderer();
    }
    
    void Update()
    {
        _time += Time.deltaTime;

        if (_time < interval) return;

        GameManager.Instance.GainEnergy(energy);
        _time = 0;
    }

    void UpdateLineRenderer()
    {
        GameObject nearestCapacitor = GetNearestCapacitor();

        if (nearestCapacitor == null)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, connectToCore ? Vector3.zero : transform.position);
            _lineRenderer.SetPosition(1, transform.position);   
        }
        else
        {
            _lineRenderer.positionCount = 3;
            _lineRenderer.SetPosition(0, connectToCore ? Vector3.zero : transform.position);
            _lineRenderer.SetPosition(1, transform.position);
            _lineRenderer.SetPosition(2, nearestCapacitor.transform.position);
        }

        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth = width;
    }
    
    private GameObject GetNearestCapacitor()
    {
        List<GameObject> capacitorsInRange = GetCapacitorsInRange();
        if (capacitorsInRange.Count == 0) return null;
            
        GameObject capacitor = capacitorsInRange
            .OrderBy(e => (e.transform.position - transform.position).sqrMagnitude)
            .First();

        return capacitor;
    }

    
    private List<GameObject> GetCapacitorsInRange()
    {
        float range = 5f;
        
        List<GameObject> output = new();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Building") &&
                hitCollider.GetComponent<Building>().BuildingSO.name.StartsWith("Capacitor"))
                output.Add(hitCollider.gameObject);
        }

        return output;
    }
}
