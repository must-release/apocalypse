using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewLoading", menuName = "Event/LoadingEvent", order = 0)]
public class LoadingEvent : IEvent
{
    public void Initialize(IEvent nextEvent)
    {
        EventType = TYPE.LOADING;
        NextEvent = nextEvent;
    }
}
