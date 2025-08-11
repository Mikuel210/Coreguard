using UnityEngine;

public class Building : MonoBehaviour
{
    [field: SerializeField] public BuildingSO BuildingSO { get; private set; }

    void OnDestroy()
    {
        Grid grid = BuildingSystem.Instance.Grid;
        
        (int gridX, int gridY) = grid.GetXY(transform.position);
        Vector2 dimensions = BuildingSO.dimensions;
        
        for (int x = gridX; x < gridX + dimensions.x; x++)
        {
            for (int y = gridY; y < gridY + dimensions.y; y++)
                grid.SetValue(x, y, 0);
        }
    }
}
