using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Scriptable Objects/Event")]
public class EventSO : ScriptableObject
{
    public event Action OnInvoked;

    public void Invoke() => OnInvoked?.Invoke();
}
