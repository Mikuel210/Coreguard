using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class BuildingSystem : Singleton<BuildingSystem>
{
    public Grid Grid { get; private set; }
    
    [field: SerializeField] public List<BuildingSO> Buildings { get; private set; }
    [SerializeField] private GameObject _buildingPlaceholderPrefab;
    
    private static bool _placingBuilding;
    private static BuildingSO _buildingBeingPlaced;
    private static GameObject _buildingPlaceholder;
    
    void Start()
    {
        int dimensions = 100;
        
        Vector3 origin = new Vector3(1, 1) * dimensions * -0.5f;
        Grid = new(dimensions, dimensions, 1, origin);
    }

    void Update()
    {
        UpdateBuildingPlaceholder();   
    }

    void UpdateBuildingPlaceholder()
    {
        if (!_placingBuilding) return;

        // Update placeholder
        Vector2 dimensions = _buildingBeingPlaced.dimensions;
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 worldPosition = Grid.SnapToGrid(mousePosition) + new Vector3(0.5f * dimensions.x, 0.5f * dimensions.y);
        _buildingPlaceholder.transform.position = worldPosition;

        // Check ability to place
        bool canPlace = CanPlace(mousePosition);
        
        // Update placeholder color
        Renderer placeholderRenderer = _buildingPlaceholder.GetComponent<Renderer>();

        if (canPlace)
            placeholderRenderer.material.color = new Color(1, 1, 1, placeholderRenderer.material.color.a);
        else
            placeholderRenderer.material.color = new Color(1, 0, 0, placeholderRenderer.material.color.a);

        // Stop building on escape
        if (Input.GetKeyDown(KeyCode.Escape))
            StopBuilding();

        // Build on click
        if (!Input.GetMouseButton(0)) return;
        if (!canPlace) return;
        
        SetBuilding(_buildingBeingPlaced, mousePosition);
        Instantiate(_buildingBeingPlaced.prefab, worldPosition, Quaternion.identity);
        
        GameManager.Instance.LoseEnergy(_buildingBeingPlaced.energyCost + _buildingBeingPlaced.increasedCost);
        _buildingBeingPlaced.increasedCost += _buildingBeingPlaced.costIncrease;
    }

    private bool CanPlace(Vector3 worldPosition)
    {
        Collider2D coreCollider = CoreController.Instance.GetComponent<Collider2D>();
        Collider2D placeholderCollider = _buildingPlaceholder.GetComponent<Collider2D>();
        bool intersectsCore = coreCollider.bounds.Intersects(placeholderCollider.bounds);
        
        bool canAfford = _buildingBeingPlaced.energyCost + _buildingBeingPlaced.increasedCost <= GameManager.Instance.Energy;
        bool intersectsBuilding = false;

        (int gridX, int gridY) = Grid.GetXY(worldPosition);
        Vector2 dimensions = _buildingBeingPlaced.dimensions;

        for (int x = gridX; x < gridX + dimensions.x; x++)
        {
            for (int y = gridY; y < gridY + dimensions.y; y++)
            {
                if (Grid.GetValue(x, y) == 0) continue;
                
                intersectsBuilding = true;
                break;
            }
            
            if (intersectsBuilding) break;
        }
        
        return !intersectsBuilding && canAfford && !intersectsCore;
    }

    private void SetBuilding(BuildingSO building, Vector3 worldPosition)
    {
        int gridValue = Buildings.IndexOf(building) + 1;
        
        (int gridX, int gridY) = Grid.GetXY(worldPosition);
        Vector2 dimensions = building.dimensions;
        
        for (int x = gridX; x < gridX + dimensions.x; x++)
        {
            for (int y = gridY; y < gridY + dimensions.y; y++)
                Grid.SetValue(x, y, gridValue);
        }
    }
    
    public void StartBuilding(BuildingSO building)
    {
        if (_placingBuilding) return;
        
        _placingBuilding = true;
        _buildingBeingPlaced = building;
        
        _buildingPlaceholder = Instantiate(_buildingPlaceholderPrefab);
        _buildingPlaceholder.transform.localScale = building.dimensions;
        _buildingPlaceholder.GetComponent<SpriteRenderer>().sprite = building.sprite;
    }

    public void StopBuilding()
    {
        Destroy(_buildingPlaceholder);
        _placingBuilding = false;
        _buildingBeingPlaced = null;
        _buildingPlaceholder = null;
    }
}
