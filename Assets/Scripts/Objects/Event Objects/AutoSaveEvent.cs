using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewAutoSave", menuName = "Event/AutoSaveEvent", order = 0)]
public class AutoSaveEvent : IEvent
{
    public void Initialize(IEvent nextEvent)
    {
        EventType = TYPE.AUTO_SAVE;
        NextEvent = nextEvent;
    }
}
