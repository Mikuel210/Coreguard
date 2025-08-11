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
        int cost = (building.energyCost + building.increasedCost);

        if (cost < 10_000)
            _text.text = cost.ToString();
        else
            _text.text = cost / 1_000f + "K";
    }
}
