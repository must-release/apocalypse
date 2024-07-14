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
        EventBase parentEvent = GameEventManager.Instance.EventPointer;
        var currentUI = UIController.Instance.GetCurrentUI();
        bool result = checkingEvent.CheckCompatibility(parentEvent, currentUI);

        return result;
    }
}
