using Helpers;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int _energy;
    public int Energy
    {
        get => _energy;
        
        private set
        {
            _energy = value;
            energyChangedEventBus.Invoke();
        }
    }
    
    [SerializeField] private EventBus energyChangedEventBus;
    
    public void GainEnergy(int energy) => Energy += energy;
}
