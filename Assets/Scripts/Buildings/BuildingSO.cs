using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Scriptable Objects/Building")]
public class BuildingSO : ScriptableObject
{
    public int energyCost;
    public int costIncrease;
    public int increasedCost;
    public Vector2 dimensions;
    
    [Space] public Sprite sprite;
    public GameObject prefab;
}
