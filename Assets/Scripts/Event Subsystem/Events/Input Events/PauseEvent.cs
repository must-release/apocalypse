using UnityEngine;
using StageEnums;
using UIEnums;
using EventEnums;
using System.Collections.Generic;

public class PauseEvent : InputEvent, KeySettingsObserver
{
    public void Start()
    {
        SettingsManager.Instance.AddObserver(this);
    }

    // Update key binding
    public void KeySettingsUpdated()
    {
        eventButton = SettingsManager.Instance.KeySettingInfo.pauseButton.buttonKeyCode;
    }

    // Check compatibiliry with event list and current UI
    public override bool CheckCompatibility(List<InputEvent> eventList, BASEUI baseUI, SUBUI subUI)
    {
        bool isEventListEmpty = eventList.Count == 0;
        bool isValidBaseUI = baseUI == BASEUI.CONTROL || baseUI == BASEUI.STORY;
        bool isValidSubUI = subUI == SUBUI.CHOICE || subUI == SUBUI.NONE;

        return isEventListEmpty && isValidBaseUI && isValidSubUI;
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
