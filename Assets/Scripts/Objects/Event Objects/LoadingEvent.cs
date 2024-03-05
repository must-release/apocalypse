using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewLoading", menuName = "Event/LoadingEvent", order = 0)]
public class LoadingEvent : ScriptableObject, IEvent
{
    public IEvent.TYPE EventType { get; set; } = IEvent.TYPE.LOADING;
    public IEvent NextEvent { get => (IEvent)nextEvent; set => nextEvent = (ScriptableObject)value; }
    public ScriptableObject nextEvent;
}
