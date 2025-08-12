using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSystem : Singleton<BuildingSystem>
{
    public Grid Grid { get; private set; }
    
    [field: SerializeField] public List<BuildingSO> Buildings { get; private set; }
    [SerializeField] private GameObject _buildingPlaceholderPrefab;
    
    public static bool PlacingBuilding { get; private set; }
    private static BuildingSO _buildingBeingPlaced;
    private static GameObject _buildingPlaceholder;
    
    public event Action OnBuildingPlacedOrDestroyed;
    
    public static bool DeletingBuildings { get; private set; }
    
    void Start()
    {
        int dimensions = 100;
        
        Vector3 origin = new Vector3(1, 1) * dimensions * -0.5f;
        Grid = new(dimensions, dimensions, 1, origin);
    }

    void Update()
    {
        UpdateBuildingPlacement();  
        UpdateBuildingRemoval();
    }

    void UpdateBuildingPlacement()
    {
        if (!PlacingBuilding) return;

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
        
        OnBuildingPlacedOrDestroyed?.Invoke();
    }
    
    private static List<GameObject> GetGameObjectsUnderMouse()
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(mouseWorldPosition, Vector2.one * 0.1f, 0);

        if (colliders.Length == 0) return new();

        // Sort by sorting layer
        Array.Sort(colliders,
            (a, b) => a.gameObject.GetComponent<Renderer>()
                .sortingLayerID.CompareTo(b.gameObject.GetComponent<Renderer>().sortingLayerID));

        // Return the GameObjects
        return colliders.Select(e => e.gameObject).ToList();
    }

    private GameObject _previousBuildingUnderMouse;

    void UpdateBuildingRemoval()
    {
        if (!DeletingBuildings) return;

        GameObject buildingUnderMouse = GetGameObjectsUnderMouse().FirstOrDefault(e => e.CompareTag("Building"));

        if (_previousBuildingUnderMouse != buildingUnderMouse && _previousBuildingUnderMouse != null)
            _previousBuildingUnderMouse.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
        
        if (!buildingUnderMouse) return;
        
        buildingUnderMouse.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
        _previousBuildingUnderMouse = buildingUnderMouse;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopDeleting();
            buildingUnderMouse.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
            
            return;
        }
        
        if (!Input.GetMouseButton(0)) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Destroy(buildingUnderMouse);
        OnBuildingPlacedOrDestroyed?.Invoke();
    }

    private bool CanPlace(Vector3 worldPosition)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return false;
        
        Collider2D coreCollider = CoreController.Instance.GetComponent<Collider2D>();
        Collider2D placeholderCollider = _buildingPlaceholder.GetComponent<Collider2D>();
        bool intersectsCore = coreCollider.bounds.Intersects(placeholderCollider.bounds);

        if (intersectsCore) return false;
        
        bool canAfford = _buildingBeingPlaced.energyCost + _buildingBeingPlaced.increasedCost <= GameManager.Instance.Energy;

        if (!canAfford) return false;
        
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
        
        return !intersectsBuilding;
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
        if (PlacingBuilding) StopBuilding();
        
        StopDeleting();
        PlacingBuilding = true;
        _buildingBeingPlaced = building;
        
        _buildingPlaceholder = Instantiate(_buildingPlaceholderPrefab);
        _buildingPlaceholder.transform.localScale = building.dimensions;
        _buildingPlaceholder.GetComponent<SpriteRenderer>().sprite = building.sprite;
    }

    public void StopBuilding()
    {
        Destroy(_buildingPlaceholder);
        PlacingBuilding = false;
        _buildingBeingPlaced = null;
        _buildingPlaceholder = null;
    }

    public void StartDeleting()
    {
        if (DeletingBuildings) return;
        
        StopBuilding();
        DeletingBuildings = true;
    }

    public void StopDeleting()
    {
        DeletingBuildings = false;
    }
}
