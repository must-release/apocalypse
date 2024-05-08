using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewLoading", menuName = "Event/LoadingEvent", order = 0)]
public class LoadingEvent : EventBase
{
    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.LOADING;
    }
}
