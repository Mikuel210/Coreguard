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
    
    void Start()
    {
        UpdatePauseMenu();
        UpdateSettingsMenu();
    }
    
    void Update()
    {
        if (!Input.GetMouseButtonDown(1)) return;
        if (!IsPaused) return;
        
        if (AreSettingsOpen)
            CloseSettings();
        else
            IsPaused = false;
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
    
    public void TogglePause() => IsPaused = !IsPaused;
    
    public void OpenSettings() => AreSettingsOpen = true;
    public void CloseSettings() => AreSettingsOpen = false;

    public void Quit()
    {
        Time.timeScale = 1f;
        GameManager.Instance.LoadScene(0);
    }
}
