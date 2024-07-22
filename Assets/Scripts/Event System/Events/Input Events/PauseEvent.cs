using UnityEngine;
using StageEnums;
using UIEnums;
using EventEnums;
using System.Collections.Generic;

public class PauseEvent : InputEvent
{
    // Check compatibiliry with event list and current UI
    public override bool CheckCompatibility(List<InputEvent> eventList, (BASEUI, SUBUI) currentUI)
    {
        bool isEventListEmpty = eventList.Count == 0;
        bool isValidUI = currentUI.Item1 == BASEUI.CONTROL || currentUI.Item1 == BASEUI.STORY || currentUI.Item2 == SUBUI.CHOICE;

        return isEventListEmpty && isValidUI;
    }

    // Play cancel event
    public override void PlayEvent()
    {
        // Turn sub UI on
        UIController.Instance.TurnSubUIOn(SUBUI.PAUSE);

        // Terminate pause event
        InputEventManager.Instance.TerminateInputEvent(this);
    }
}
