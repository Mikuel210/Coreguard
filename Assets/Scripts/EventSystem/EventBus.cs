using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventBus
{
    [SerializeField] private List<EventSO> events;
    
    public void Invoke() => events.ForEach(e => e.Invoke());
}
