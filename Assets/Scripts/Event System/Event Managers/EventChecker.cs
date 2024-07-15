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

    // Check compatibility of the new game event with current event
    public bool CheckEventCompatibility(GameEvent checkingEvent)
    {
        GameEvent parentEvent = GameEventManager.Instance.EventPointer;
        var currentUI = UIController.Instance.GetCurrentUI();
        bool result = checkingEvent.CheckCompatibility(parentEvent, currentUI);

        return result;
    }

    // Check compatibility of the new input event with current event
    public bool CheckEventCompatibility(InputEvent checkingEvent)
    {
        List<InputEvent> eventList = InputEventManager.Instance.EventList;
        var currentUI = UIController.Instance.GetCurrentUI();
        bool result = checkingEvent.CheckCompatibility(eventList, currentUI);

        return result;
    }
}
