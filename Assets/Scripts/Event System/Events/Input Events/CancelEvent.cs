using UnityEngine;
using StageEnums;
using UIEnums;
using EventEnums;

public class CancelEvent : InputEvent
{
    // Check compatibiliry with parent event and current UI
    public override bool CheckCompatibility(InputEvent parentEvent, (BASEUI, SUBUI) currentUI)
    {
        bool isParentEventNull = parentEvent == null;
        bool isInvalidSubUI = currentUI.Item2 == SUBUI.NONE || currentUI.Item2 == SUBUI.CHOICE || currentUI.Item2 == SUBUI.SAVING;

        return isParentEventNull && !isInvalidSubUI;
    }

    // Play cancel event
    public override void PlayEvent()
    {
        UIController.Instance.CancelCurrentUI();
    }
}
