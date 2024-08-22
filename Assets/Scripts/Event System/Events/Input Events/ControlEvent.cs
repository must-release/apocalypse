using System.Collections;
using System.Collections.Generic;
using UIEnums;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlEvent : InputEvent, KeySettingsObserver
{
    private KeyCode 
    upButton, rightButton, leftButton, downButton, jumpButton, attackButton, 
    assistAttackButton, aimButton, specialAttackButton, tagButton;

    public void Start()
    {
        SettingsManager.Instance.AddObserver(this);
    }

    // Update key binding
    public void KeySettingsUpdated()
    {
        var keySettings = SettingsManager.Instance.KeySettingInfo;
        upButton = keySettings.upButton.buttonKeyCode;
        rightButton = keySettings.rightButton.buttonKeyCode;
        leftButton = keySettings.leftButton.buttonKeyCode;
        downButton = keySettings.downButton.buttonKeyCode;
        jumpButton = keySettings.jumpButton.buttonKeyCode;
        attackButton = keySettings.attackButton.buttonKeyCode;
        assistAttackButton = keySettings.assistAttackButton.buttonKeyCode;
        aimButton = keySettings.aimButton.buttonKeyCode;
        specialAttackButton = keySettings.specialAttackButton.buttonKeyCode;
        tagButton = keySettings.tagButton.buttonKeyCode;
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
