using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private EventSO energyUpdatedEvent;
    [SerializeField] private TextMeshProUGUI _energyText;
    
    [SerializeField] private EventSO capacitanceUpdatedEvent;
    [SerializeField] private TextMeshProUGUI _capacitanceText;
    
    [SerializeField] private EventSO stateChangedEvent;
    [Space, SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _stateTimeText;

    [Space, SerializeField] private GameObject _shopPanel;
    [SerializeField] private GameObject _shopButton;
    [SerializeField] private GameObject _deleteButton;
    
    [Space, SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    
    [Space, SerializeField] private List<TextMeshProUGUI> waveTexts;
    [SerializeField] private List<TextMeshProUGUI> timeTexts;

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
        energyUpdatedEvent.OnInvoked += UpdateEnergyText;
        capacitanceUpdatedEvent.OnInvoked += UpdateCapacitanceText;
        stateChangedEvent.OnInvoked += UpdateStatePanels;
        
        UpdateShop();
        UpdateEnergyText();
        UpdateCapacitanceText();
    }

    void Update()
    {
        // Close shop on escape
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseShop();
        
        // Update state time
        UpdateStateTime();
        UpdateWaveText();
    }

    public void OpenShop() => IsShopOpen = true;
    public void CloseShop() => IsShopOpen = false;

    void UpdateShop()
    {
        if (_isShopOpen)
        {
            _shopPanel.SetActive(true);
            _shopButton.SetActive(false);       
            _deleteButton.SetActive(false);
        }
        else
        {
            _shopPanel.SetActive(false);
            _shopButton.SetActive(true);
            _deleteButton.SetActive(true);
        }
    }
    
    void UpdateStatePanels()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.Won)
            winPanel.SetActive(true);
        else if (GameManager.Instance.CurrentState == GameManager.GameState.Lost)
            losePanel.SetActive(true);
        
        foreach (TextMeshProUGUI text in waveTexts)
            text.text = "WAVE " + GameManager.Instance.CurrentWaveIndex;

        int time = Mathf.FloorToInt(Time.timeSinceLevelLoad);
        
        int seconds = time % 60;
        int minutes = time / 60;

        foreach (TextMeshProUGUI text in timeTexts)
            text.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    void UpdateStateTime()
    {
        if (GameManager.Instance.HiddenByTutorial || GameManager.Instance.CurrentWaveIndex >= GameManager.Instance.Waves.Count)
        {
            _stateTimeText.gameObject.SetActive(false);
            return;
        }
        
        _stateTimeText.gameObject.SetActive(true);
        
        string prefix = GameManager.Instance.CurrentState == GameManager.GameState.Intermission ? "T-" : "T+";

        int time = Mathf.FloorToInt(GameManager.Instance.StateTime);

        if (GameManager.Instance.CurrentState == GameManager.GameState.Intermission)
            time = Mathf.FloorToInt(GameManager.Instance.CurrentWave.delay) - time;
        
        int seconds = time % 60;
        int minutes = time / 60;
        
        string text = prefix + minutes.ToString("00") + ":" + seconds.ToString("00");
        _stateTimeText.text = text;
    }

    void UpdateWaveText()
    {
        if (GameManager.Instance.HiddenByTutorial || GameManager.Instance.CurrentWaveIndex >= GameManager.Instance.Waves.Count)
            _waveText.gameObject.SetActive(false);
        else
        { 
            _waveText.gameObject.SetActive(true);
            _waveText.text = "WAVE " + (GameManager.Instance.CurrentWaveIndex + 1);
        }
    }

    void UpdateEnergyText()
    {
        _energyText.text = GameManager.Instance.Energy.ToString();
        Vector2 preferredSize = _energyText.GetPreferredValues();
        _energyText.rectTransform.sizeDelta = preferredSize;
    }

    void UpdateCapacitanceText()
    {
        _capacitanceText.text = "/" + GameManager.Instance.Capacitance;
        
        Vector2 preferredSize = _capacitanceText.GetPreferredValues();
        _capacitanceText.rectTransform.sizeDelta = new(preferredSize.x, _capacitanceText.rectTransform.sizeDelta.y);
    }

    public void Buy(string buildingName)
    {
        BuildingSO building = BuildingSystem.Instance.Buildings.FirstOrDefault(e => e.name == buildingName);

        if (!building) return;
        
        BuildingSystem.Instance.StartBuilding(building);
        CloseShop();
    }
}
