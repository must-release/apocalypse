using System.Collections;
using System.Collections.Generic;
using UIEnums;
using UnityEngine;

public class NextScriptEvent : InputEvent
{
    // Check compatibiliry with event list and current UI
    public override bool CheckCompatibility(List<InputEvent> eventList, BASEUI baseUI, SUBUI subUI)
    {
        bool isEventListEmpty = eventList.Count == 0;
        bool isValidUI = baseUI == BASEUI.STORY && subUI == SUBUI.NONE;

        return isEventListEmpty && isValidUI;
    }

    // Play pause event
    public override void PlayEvent()
    {
        // Play next story script
        StoryController.Instance.PlayNextScript();

        // Terminate pause event
        InputEventManager.Instance.TerminateInputEvent(this);
    }
}
