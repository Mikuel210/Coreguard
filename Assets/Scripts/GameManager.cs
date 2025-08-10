using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int energy;
    public int Energy
    {
        get => energy;
        
        private set
        {
            energy = value;
            energyChangedEventBus.Invoke();
        }
    }
    
    [SerializeField] private EventBus energyChangedEventBus;

    private int _currentWaveIndex;
    public int CurrentWaveIndex
    {
        get => _currentWaveIndex;

        private set
        {
            _currentWaveIndex = value;

            try {
                CurrentWave = Waves[_currentWaveIndex];    
            } catch { }
            
            
            _currentWaveDuration = 0;
            
            foreach (Burst burst in CurrentWave.bursts)
                _currentWaveDuration += burst.delay * burst.amount;
            
            waveChangedEventBus.Invoke();
        }
    }
    
    [SerializeField] private EventBus waveChangedEventBus;

    public Wave CurrentWave { get; private set; }
    private float _currentWaveDuration;
    
    
    [Serializable]
    public class Burst
    {
        public GameObject prefab;
        public int amount;
        public float delay;
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
        SpawningWave
    }

    public GameState CurrentState { get; private set; } = GameState.Intermission;
    
    public float StateTime { get; private set; }

    void Start() => CurrentWaveIndex = 0;
    
    void Update()
    {
        StateTime += Time.deltaTime;
        
        if (CurrentState == GameState.Intermission)
        {
            if (StateTime < CurrentWave.delay) return;
            
            StartCoroutine(SpawnWave());
            
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
    
    
    private IEnumerator SpawnWave()
    {
        foreach (Burst burst in CurrentWave.bursts)
            yield return SpawnBurst(burst);
    }    
    
    private Random _random = new();
    private IEnumerator SpawnBurst(Burst burst)
    {
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
}
