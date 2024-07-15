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

<<<<<<< HEAD
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
        InputEvent parentEvent = InputEventManager.Instance.EventPointer;
        var currentUI = UIController.Instance.GetCurrentUI();
        bool result = checkingEvent.CheckCompatibility(parentEvent, currentUI);
=======
    // Check compatibility of the input event with current event
    public bool CheckEventCompatibility(EventBase checkingEvent)
    {
        EventBase headEvent = GameEventManager.Instance.HeadEvent;
        bool result = checkingEvent.CheckCompatibility(headEvent);
>>>>>>> origin/minjung

        return result;
    }
}
