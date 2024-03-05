using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewInGame", menuName = "Event/InGameEvent", order = 0)]
public class InGameEvent : ScriptableObject, IEvent
{
    public IEvent.TYPE EventType { get; set; } = IEvent.TYPE.IN_GAME;
    public IEvent NextEvent { get => (IEvent)nextEvent; set => nextEvent = (ScriptableObject)value; }
    public ScriptableObject nextEvent;

}
