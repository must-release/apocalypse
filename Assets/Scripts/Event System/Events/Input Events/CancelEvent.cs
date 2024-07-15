using UnityEngine;
using StageEnums;
using UIEnums;
using System.Collections.Generic;
using EventEnums;

public class CancelEvent : InputEvent
{
    // Check compatibiliry with parent event and current UI
    public override bool CheckCompatibility(List<InputEvent> eventList, (BASEUI, SUBUI) currentUI)
    {
        bool isEventListEmpty = eventList.Count == 0;
        bool isInvalidSubUI = currentUI.Item2 == SUBUI.NONE || currentUI.Item2 == SUBUI.CHOICE || currentUI.Item2 == SUBUI.SAVING;

        return isEventListEmpty && !isInvalidSubUI;
    }

    // Play cancel event
    public override void PlayEvent()
    {
        UIController.Instance.CancelCurrentUI();
        InputEventManager.Instance.TerminateEvent(this);
    }
}
