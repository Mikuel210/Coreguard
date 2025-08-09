using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private EventSO energyUpdatedEvent;
    private TextMeshProUGUI _energyText;

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
        _energyText = GetComponent<TextMeshProUGUI>();
        
        energyUpdatedEvent.OnInvoked += () =>
            _energyText.text = GameManager.Instance.Energy.ToString();
        
        UpdateShop();
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
}
