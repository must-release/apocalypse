using UnityEngine;
using StageEnums;
using UIEnums;
using EventEnums;
using System.Collections.Generic;

public class PauseEvent : InputEvent
{
    // Check compatibiliry with event list and current UI
    public override bool CheckCompatibility(List<InputEvent> eventList, BASEUI baseUI, SUBUI subUI)
    {
        bool isEventListEmpty = eventList.Count == 0;
        bool isValidUI = baseUI == BASEUI.CONTROL || baseUI == BASEUI.STORY || subUI == SUBUI.CHOICE;

        return isEventListEmpty && isValidUI;
    }

    // Play pause event
    public override void PlayEvent()
    {
        // Turn sub UI on
        UIController.Instance.TurnSubUIOn(SUBUI.PAUSE);

        // Terminate pause event
        InputEventManager.Instance.TerminateInputEvent(this);
    }
}
