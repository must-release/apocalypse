using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewInGame", menuName = "Event/InGameEvent", order = 0)]
public class InGameEvent : EventBase
{
    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.IN_GAME;
    }
}
