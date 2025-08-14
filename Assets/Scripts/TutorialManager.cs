using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;
using TMPro;

public class TutorialManager : Singleton<TutorialManager>
{
    [field: SerializeField] public int MessageIndex { get; private set; }

    private List<string> _messages = new()
    {
        "This is the Core. Nobody remembers building it. Nobody remembers anything before it. We only know one truth: it holds the power of creation itself. If it ever falls into the wrong hands, reality as we know it will collapse.", 
        "The Core can turn your thoughts into Raw Energy, a substance that can be shaped into anything you imagine.",
        "Click on the Core to turn your thoughts into Raw Energy. Gather 50 units to continue.",
        "Now, let's make your first Dream Condenser: it draws fragments of dreams from the air and turns them into Raw Energy.",
        "Open the shop in the bottom left, select the condenser, use WASD to move around and click to place it wherever you like. Finally, press Escape to stop placing.",
        "Keep clicking the Core and placing Dream Condensers until you gather 500 units of Raw Energy.",
        "Place a Tier II Dream Condenser. It generates 15 units of Raw Energy per second.",
        "Your energy storage is limited. If you want to progress any further, you'll have to build Energy Vaults. Place enough of them to store 2,500 units of Raw Energy.",
        "Gather 2,500 units of Raw Energy, then place an Energy Harvester: it draws power directly from the Core, producing immense quantities of energy.",
        "Creation alone isn't enough. Enemies will come. Build at least 4 weapons to guard the Core from all directions.",
        "Finally, shield your structures. Place walls until the Core and key buildings are protected on all sides. Enemies are right around the corner.",
    };

    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;

    void Start() => StartCoroutine(StartTutorial());

    private IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(2);
        ShowMessage();
    }

    void Update()
    {
        switch (MessageIndex)
        {
            case 3:
                if (GameManager.Instance.Energy < 50) return;
                break;
            
            case 5:
                if (!FindBuilding("Condenser1")) return;
                if (BuildingSystem.PlacingBuilding) return;

                break;
            
            case 6:
                if (GameManager.Instance.Energy < 500) return;
                break;
            
            case 7:
                if (!FindBuilding("Condenser2")) return;
                if (BuildingSystem.PlacingBuilding) return;

                break;
            
            case 8:
                if (GameManager.Instance.Capacitance < 2500) return;
                if (BuildingSystem.PlacingBuilding) return;
                
                break;
            
            case 9:
                if (!FindBuilding("Harvester1")) return;
                if (BuildingSystem.PlacingBuilding) return;

                break;
            
            case 10:
                List<Building> buildings = FindObjectsByType<Building>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
                if (buildings.Count == 0) return;

                int count = 0;

                foreach (Building building in buildings)
                {
                    string[] weaponNames = { "Shotgun1", "Shotgun2", "Shotgun3", "Shotgun4", "MachineGun1", "MachineGun2", "MachineGun3" };

                    if (weaponNames.Contains(building.BuildingSO.name))
                        count++;
                }

                if (count < 4) return;
                if (BuildingSystem.PlacingBuilding) return;

                break;
            
            default:
                return;
        }
        
        ShowMessage();
    }

    private bool FindBuilding(string name)
    {
        List<Building> buildings = FindObjectsByType<Building>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        if (buildings.Count == 0) return false;
        
        bool found = false;

        foreach (Building building in buildings)
        {
            if (building.BuildingSO.name == name)
                found = true;

            if (found) break;
        }
        
        return found;
    }

    private void Advance() => MessageIndex++;

    public void ShowMessage()
    {
        messagePanel.SetActive(true);
        messageText.text = _messages[MessageIndex] + " ";
    }

    private void HideMessage()
    {
        messagePanel.SetActive(false);

        if (MessageIndex != 11) return;

        GameManager.Instance.HiddenByTutorial = false;
    }

    public void Continue()
    {
        switch (MessageIndex)
        {
            case 0:
            case 1:
            case 3:
                Advance();
                ShowMessage();

                break;
            
            default:
                Advance();
                HideMessage();
                
                break;
        }
    }
}
