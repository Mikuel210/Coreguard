using Helpers;
using UnityEngine;

public class PauseSystem : Singleton<PauseSystem>
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    
    private bool _isPaused = false;
    public bool IsPaused
    {
        get => _isPaused;
        
        private set
        {
            _isPaused = value;
            UpdatePauseMenu();
        }
    }
    
    private bool _areSettingsOpen = false;
    public bool AreSettingsOpen
    {
        get => _areSettingsOpen;
        
        private set
        {
            _areSettingsOpen = value;
            UpdateSettingsMenu();
        }
    }
    
    private bool _dontPauseThisFrame;
    
    void Start()
    {
        UpdatePauseMenu();
        UpdateSettingsMenu();
    }
    
    void Update()
    {
        if (_dontPauseThisFrame)
        {
            _dontPauseThisFrame = false;
            return;
        }
        
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (!IsPaused)
        {
            if (BuildingSystem.PlacingBuilding || BuildingSystem.DeletingBuildings) return;
            if (UIManager.Instance.IsShopOpen) return;
            
            if (GameManager.Instance.CurrentState == GameManager.GameState.Won ||
                GameManager.Instance.CurrentState == GameManager.GameState.Lost) return;
            
            IsPaused = true;
        }
        else
        {
            if (AreSettingsOpen)
                CloseSettings();
            else
                IsPaused = false;
        }
    }

    void UpdatePauseMenu()
    {
        if (IsPaused)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void UpdateSettingsMenu()
    {
        if (AreSettingsOpen)
        {
            settingsMenu.SetActive(true);
        }
        else
        {
            settingsMenu.SetActive(false);
        }
    }
    
    public void ResumeGame() => IsPaused = false;
    
    public void OpenSettings() => AreSettingsOpen = true;
    public void CloseSettings() => AreSettingsOpen = false;

    public void DontPauseThisFrame() => _dontPauseThisFrame = true;

    public void Quit()
    {
        Time.timeScale = 1f;
        GameManager.Instance.LoadScene(0);
    }
}
