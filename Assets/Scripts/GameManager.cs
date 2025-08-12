using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

public class GameManager : Singleton<GameManager>
{
    [field: SerializeField] public bool IsTutorial { get; private set; }
    
    [Space, SerializeField] private int capacitance;
    public int Capacitance
    {
        get => capacitance;
        
        private set
        {
            capacitance = value;
            OnCapacitanceChanged?.Invoke();
            Energy = Energy;
        }    
    }
    
    public event Action OnCapacitanceChanged;
    
    [SerializeField] private int energy;
    public int Energy
    {
        get => energy;
        
        private set
        {
            energy = Mathf.Min(value, capacitance);
            OnEnergyChanged?.Invoke();
        }
    }
    
    public event Action OnEnergyChanged;

    private bool _checkWin;

    private int _currentWaveIndex;
    public int CurrentWaveIndex
    {
        get => _currentWaveIndex;

        private set
        {
            _currentWaveIndex = value;

            try {
                CurrentWave = Waves[_currentWaveIndex];
            }
            catch { }
        }
    }
    
    public event Action OnStateChanged;

    public Wave CurrentWave { get; private set; }
    private float _currentWaveDuration;
    
    
    [Serializable]
    public class Burst
    {
        public GameObject prefab;
        public int amount;
        public float delay;
        public float initialDelay;
    }

    [Serializable]
    public class Wave
    {
        public List<Burst> bursts;
        public float delay;
    }
    
    [field: SerializeField] public List<Wave> Waves { get; private set; }


    public enum GameState
    {
        Intermission,
        SpawningWave,
        Won,
        Lost
    }

    private GameState _currentState = GameState.Intermission;

    public GameState CurrentState
    {
        get => _currentState;

        private set
        {
            _currentState = value;
            
            if (_currentState == GameState.SpawningWave)
                LatestStartedWave++;
            
            OnStateChanged?.Invoke();
        }
    }

    public float StateTime { get; private set; }
    
    private bool _hiddenByTutorial;
    public bool HiddenByTutorial
    {
        get => _hiddenByTutorial;
        
        set
        {
            _hiddenByTutorial = value;
            OnStateChanged?.Invoke();
        }
    }

    
    public int LatestStartedWave { get; private set; }
    
    
    void Start()
    {
        _hiddenByTutorial = IsTutorial;
        CurrentWaveIndex = 0;
        CoreController.Instance.GetComponent<Health>().OnDeath += Lose;
    }
    
    void Update()
    {
        if (IsTutorial && HiddenByTutorial) return;
        
        StateTime += Time.deltaTime;
        
        if (CurrentState == GameState.Intermission)
        {
            if (CurrentWaveIndex == Waves.Count && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
                Win();
            
            if (StateTime < CurrentWave.delay) return;
            
            SpawnWave();
            
            CurrentState = GameState.SpawningWave;
            StateTime = 0;
        } 
        else if (CurrentState == GameState.SpawningWave)
        {
            if (StateTime < _currentWaveDuration) return;
            
            CurrentWaveIndex += 1;
            
            CurrentState = GameState.Intermission;
            StateTime = 0;
        }
    }
    
    
    private void SpawnWave()
    {
        _currentWaveDuration = 0;

        foreach (Burst burst in CurrentWave.bursts)
        {
            StartCoroutine(SpawnBurst(burst));   
            _currentWaveDuration = Mathf.Max(_currentWaveDuration, burst.initialDelay + burst.delay * burst.amount);
        }
    }    
    
    private Random _random = new();
    private IEnumerator SpawnBurst(Burst burst)
    {
        yield return new WaitForSeconds(burst.initialDelay);
        
        float distance = 25f;

        int angle = _random.Next(0, 360);
        float x = Mathf.Cos(angle) * distance;
        float y = Mathf.Sin(angle) * distance;

        Vector2 spawnPosition = new(x, y);

        for (int i = 0; i < burst.amount; i++)
        {
            yield return new WaitForSeconds(burst.delay);
            Instantiate(burst.prefab, spawnPosition, Quaternion.identity);
        }
    }
    
    public void GainEnergy(int energy) => Energy += energy;
    
    public void LoseEnergy(int energy) => Energy -= energy;
    
    public void AddCapacitance(int capacitance) => Capacitance += capacitance;

    public void LoseCapacitance(int capacitance) => Capacitance -= capacitance;

    public void Win() => CurrentState = GameState.Won;
    
    public void Lose() => CurrentState = GameState.Lost;
    
    
    public void LoadScene(int scene) => SceneManager.LoadScene(scene);
}
