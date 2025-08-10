using UnityEngine;

public class Grid
{
    private int _width;
    private int _height;
    private float _cellSize;
    private Vector3 _origin;
    private int[,] _gridArray;
    
    public Grid(int width, int height, float cellSize, Vector3 origin)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _origin = origin;
        
        _gridArray = new int[_width, _height];

        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < _gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 1000f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 1000f);
            }   
        }
        
        Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, 1000f);
        Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, 1000f);
    }
    
    public Vector3 GetWorldPosition(int x, int y) => new Vector3(x, y) * _cellSize + _origin;
    public (int x, int y) GetXY(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - _origin).x / _cellSize);
        int y = Mathf.FloorToInt((worldPosition - _origin).y / _cellSize);
        
        return (x, y);
    }
    
    public Vector3 SnapToGrid(Vector3 worldPosition)
    {
        (int x, int y) = GetXY(worldPosition);
        return GetWorldPosition(x, y);
    }
    
    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
            _gridArray[x, y] = value;
    }
    public void SetValue(Vector3 worldPosition, int value)
    {
        (int x, int y) = GetXY(worldPosition);
        SetValue(x, y, value);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
            return _gridArray[x, y];
        
        return 0;
    }
    public int GetValue(Vector3 worldPosition)
    {
        (int x, int y) = GetXY(worldPosition);
        return GetValue(x, y);
    }
}
