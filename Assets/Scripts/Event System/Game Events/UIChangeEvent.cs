using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewUIChange", menuName = "Event/UIChange", order = 0)]
public class UIChangeEvent : EventBase
{
    public BASEUI changingUI;

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.UI_CHANGE;
    }
}