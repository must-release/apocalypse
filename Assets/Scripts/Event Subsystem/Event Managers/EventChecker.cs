using System.Collections;
using System.Collections.Generic;
using UIEnums;
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

    // Check compatibility of the new game event with parent event
    public bool CheckEventCompatibility(GameEvent checkingEvent, GameEvent parentEvent)
    {
        UIController.Instance.GetCurrentUI(out BaseUI baseUI, out SubUI subUI);
        bool result = checkingEvent.CheckCompatibility(parentEvent, baseUI, subUI);

        if (!result)
        {
            Debug.Log("Not compatible :" + checkingEvent.EventType.ToString());
        }


        return result;
    }

    // Check compatibility of the new game event and EventPointer
    public bool CheckEventCompatibility(GameEvent checkingEvent)
    {
        return CheckEventCompatibility(checkingEvent, GameEventManager.Instance.EventPointer);
    }

    // Check compatibility of the new input event with current event
    public bool CheckEventCompatibility(InputEvent checkingEvent)
    {
        List<InputEvent> eventList = InputEventManager.Instance.EventList;
        UIController.Instance.GetCurrentUI(out BaseUI baseUI, out SubUI subUI);
        bool result = checkingEvent.CheckCompatibility(eventList, baseUI, subUI);

        return result;
    }
}
