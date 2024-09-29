using System.Collections;
using System.Collections.Generic;
using UIEnums;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlEvent : InputEvent, KeySettingsObserver
{
    private KeyCode 
    upButton, rightButton, leftButton, downButton, jumpButton, attackButton, 
    assistAttackButton, aimButton, specialAttackButton, tagButton, interactButton;

    private ControlInfo controlInfo;

    public void Start()
    {
        SettingsManager.Instance.AddObserver(this);

        controlInfo = new ControlInfo();
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
        interactButton = keySettings.interactionButton.buttonKeyCode;
    }

    // Detect if event button is pressed or panel is clicked
    public override bool DetectInput()
    {
        bool buttonClicked = SetControlInfo();
        bool aimed = SetAimControlInfo();
        return buttonClicked || aimed;
    }

    // Set control info according to pressed keys
    private bool SetControlInfo()
    {
        // Set move & stop control
        int prevMove = controlInfo.move;
        controlInfo.move = 0;
        if (Input.GetKey(rightButton)) controlInfo.move += 1;
        if (Input.GetKey(leftButton)) controlInfo.move -= 1;
        controlInfo.stop = prevMove != 0 && controlInfo.move == 0;

        // Set lookUp & climb & stop control
        int prevUpDown = controlInfo.upDown;
        controlInfo.upDown = 0;
        if(Input.GetKey(upButton)) controlInfo.upDown += 1;
        if(Input.GetKey(downButton)) controlInfo.upDown -= 1;
        bool applyUpDown = controlInfo.upDown != 0 || prevUpDown != 0;

        // Set jump control
        controlInfo.jump = Input.GetKeyDown(jumpButton);

        // Set attack control
        controlInfo.attack = Input.GetKeyDown(attackButton);
        controlInfo.assistAttack = Input.GetKeyDown(assistAttackButton);
        controlInfo.specialAttack = Input.GetKeyDown(specialAttackButton);

        // Set tag control
        controlInfo.tag = Input.GetKeyDown(tagButton);

        // Try interact control
        controlInfo.tryInteract = Input.GetKeyDown(interactButton);

        return (controlInfo.move != 0) || controlInfo.stop || applyUpDown || controlInfo.jump || 
            controlInfo.assistAttack || controlInfo.specialAttack || controlInfo.tag || controlInfo.tryInteract;
    }

    // Set aim info according to mouse position
    private bool SetAimControlInfo()
    {
        if (Input.GetKey(aimButton))
        {
            Vector3 mousePosition = Input.mousePosition; 
            controlInfo.aim = Camera.main.ScreenToWorldPoint(mousePosition);
            return true;
        }
        else return false;
    }

    // Check compatibiliry with event list and current UI
    public override bool CheckCompatibility(List<InputEvent> eventList, BASEUI baseUI, SUBUI subUI)
    {
        bool isEventListEmpty = eventList.Count == 0;
        bool isValidUI = baseUI == BASEUI.CONTROL && subUI == SUBUI.NONE;

        return isEventListEmpty && isValidUI;
    }

    // Play pause event
    public override void PlayEvent()
    {
        // Control player character according to control info
        GamePlayManager.Instance.ControlPlayerCharacter(controlInfo);

        // Terminate pause event
        InputEventManager.Instance.TerminateInputEvent(this);
    }
}
