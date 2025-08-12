using UnityEngine;

public class Building : MonoBehaviour
{
    [field: SerializeField] public BuildingSO BuildingSO { get; private set; }

    void OnDestroy()
    {
        Grid grid = BuildingSystem.Instance.Grid;
        
        Vector2 dimensions = BuildingSO.dimensions;
        (int gridX, int gridY) = grid.GetXY(transform.position - new Vector3(0.5f * dimensions.x, 0.5f * dimensions.y));
        
        for (int x = gridX; x < gridX + dimensions.x; x++)
        {
            for (int y = gridY; y < gridY + dimensions.y; y++)
                grid.SetValue(x, y, 0);
        }
    }
}
