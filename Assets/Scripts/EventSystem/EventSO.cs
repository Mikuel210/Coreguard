using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Scriptable Objects/Event")]
public class EventSO : ScriptableObject
{
    public event Action OnInvoked;

    public void Invoke()
    {
        try {
            OnInvoked?.Invoke();
        }
        catch (Exception e) {
            Debug.LogError("An exception was caught while invoking an event: " + e);
        }
    }
}
