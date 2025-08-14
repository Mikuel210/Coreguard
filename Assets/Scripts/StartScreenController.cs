using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] private List<GameObject> optionPanels;
    [SerializeField] private GameObject settingsPanel;
    
    private bool _areSettingsOpen;
    public bool AreSettingsOpen
    {
        get => _areSettingsOpen;

        private set
        {
            _areSettingsOpen = value;
            UpdateSettings();
        }
    }

    private int _optionsIndex;

    void Start()
    {
        ShowOptions(0);
        UpdateSettings();
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(1)) return;
        
        if (AreSettingsOpen)
            CloseSettings();
        else if (_optionsIndex == 1)
            ShowOptions(0);
    }
    
    
    public void LoadScene(int scene) => SceneManager.LoadScene(scene);

    public void ShowOptions(int index)
    {
        _optionsIndex = index; 
        
        for (int i = 0; i < optionPanels.Count; i++)
        {
            GameObject optionPanel = optionPanels[i];
            optionPanel.SetActive(index == i);
        }
    }

    void UpdateSettings()
    {
        settingsPanel.SetActive(AreSettingsOpen);
    }
    
    public void OpenSettings() => AreSettingsOpen = true;
    public void CloseSettings() => AreSettingsOpen = false;
}
