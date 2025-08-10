using TMPro;
using UnityEngine;

public class CostUpdater : MonoBehaviour
{
    [SerializeField] private BuildingSO building;
    private TMP_Text _text;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        building.increasedCost = 0;
        _text = transform.Find("EnergyCostText").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        _text.text = (building.energyCost + building.increasedCost).ToString();
    }
}
