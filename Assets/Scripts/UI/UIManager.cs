using System.Linq;
using Helpers;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private EventSO energyUpdatedEvent;
    [SerializeField] private TextMeshProUGUI _energyText;
    
    [SerializeField] private EventSO waveUpdatedEvent;
    [Space, SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _stateTimeText;

    [Space, SerializeField] private GameObject _shopPanel;
    [SerializeField] private GameObject _shopButton;

    private bool _isShopOpen;
    private bool IsShopOpen
    {
        get => _isShopOpen;
        
        set
        {
            _isShopOpen = value;
            UpdateShop();
        }
    }

    void Start()
    {
        energyUpdatedEvent.OnInvoked += () =>
        {
            _energyText.text = GameManager.Instance.Energy.ToString();
            UpdateEnergyText();
        };

        waveUpdatedEvent.OnInvoked += () =>
            _waveText.text = "WAVE " + (GameManager.Instance.CurrentWaveIndex + 1);
        
        UpdateShop();
        UpdateEnergyText();
    }

    void Update()
    {
        // Close shop on escape
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseShop();
        
        // Update state time
        string prefix = GameManager.Instance.CurrentState == GameManager.GameState.Intermission ? "T-" : "T+";

        int time = Mathf.FloorToInt(GameManager.Instance.StateTime);

        if (GameManager.Instance.CurrentState == GameManager.GameState.Intermission)
            time = Mathf.FloorToInt(GameManager.Instance.CurrentWave.delay) - time;
        
        int seconds = time % 60;
        int minutes = time / 60;
        
        string text = prefix + minutes.ToString("00") + ":" + seconds.ToString("00");
        _stateTimeText.text = text;
    }

    public void OpenShop() => IsShopOpen = true;
    public void CloseShop() => IsShopOpen = false;

    void UpdateShop()
    {
        if (_isShopOpen)
        {
            _shopPanel.SetActive(true);
            _shopButton.SetActive(false);       
        }
        else
        {
            _shopPanel.SetActive(false);
            _shopButton.SetActive(true);
        }
    }

    void UpdateEnergyText()
    {
        Vector2 preferredSize = _energyText.GetPreferredValues();
        _energyText.rectTransform.sizeDelta = preferredSize;
    }

    public void Buy(string buildingName)
    {
        BuildingSO building = BuildingSystem.Instance.Buildings.FirstOrDefault(e => e.name == buildingName);

        if (!building) return;
        
        BuildingSystem.Instance.StartBuilding(building);
        CloseShop();
    }
}
