using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChecker : MonoBehaviour
{
    public static EventChecker Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Check compatibility of the input event with current event
    public bool CheckEventCompatibility(EventBase checkingEvent)
    {
        EventBase headEvent = GameEventManager.Instance.EventPointer;
        bool result = checkingEvent.CheckCompatibility(headEvent);

        return result;
    }
}
