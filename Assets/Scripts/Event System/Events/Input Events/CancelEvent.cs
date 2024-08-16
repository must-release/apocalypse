using UnityEngine;
using StageEnums;
using UIEnums;
using System.Collections.Generic;
using EventEnums;

public class CancelEvent : InputEvent, KeySettingsObserver
{
    public void Start()
    {
        SettingsManager.Instance.AddObserver(this);
    }

    // Update key binding
    public void KeySettingsUpdated()
    {
        eventButton = SettingsManager.Instance.KeySettingInfo.cancelButton;
    }

    // Check compatibiliry with event list and current UI
    public override bool CheckCompatibility(List<InputEvent> eventList, BASEUI baseUI, SUBUI subUI)
    {
        bool isEventListEmpty = eventList.Count == 0;
        bool isInvalidSubUI = subUI == SUBUI.NONE || subUI == SUBUI.CHOICE || subUI == SUBUI.SAVING;

        return isEventListEmpty && !isInvalidSubUI;
    }

    // Play cancel event
    public override void PlayEvent()
    {
        // Cancel current UI
        UIController.Instance.CancelCurrentUI();

        // Terminate cancel event
        InputEventManager.Instance.TerminateInputEvent(this);
    }
}
