using System.Collections;
using System.Collections.Generic;
using UIEnums;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextScriptEvent : InputEvent, KeySettingsObserver
{
    public void Start()
    {
        SettingsManager.Instance.AddObserver(this);
    }

    // Update key binding
    public void KeySettingsUpdated()
    {
        eventButton = SettingsManager.Instance.KeySettingInfo.confirmButton.buttonKeyCode;
    }

    // Detect if event button is pressed or panel is clicked
    public override bool DetectInput()
    {
        bool buttonClicked = Input.GetKeyDown(eventButton);
        bool panelClicked = UIController.Instance.IsStoryPanelClicked;
        return buttonClicked || panelClicked;
    }

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
