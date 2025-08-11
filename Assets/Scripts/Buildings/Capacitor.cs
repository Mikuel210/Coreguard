using System;
using UnityEngine;

public class Capacitor : MonoBehaviour
{
    [SerializeField] private int capacitance;
    
    void Start() => GameManager.Instance.AddCapacitance(capacitance);
    void OnDestroy() => GameManager.Instance.LoseCapacitance(capacitance);
}
