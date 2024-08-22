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
        if (Input.GetKey(upButton)) controlInfo.move += new Vector2(0, 1);
        if (Input.GetKey(rightButton)) controlInfo.move += new Vector2(1, 0);
        if (Input.GetKey(leftButton)) controlInfo.move += new Vector2(-1, 0);
        if (Input.GetKey(downButton)) controlInfo.move += new Vector2(0, -1);
        if (Input.GetKeyDown(jumpButton)) controlInfo.jump = true;
        if (Input.GetKeyDown(attackButton)) controlInfo.attack = true;
        if (Input.GetKeyDown(assistAttackButton)) controlInfo.assistAttack = true;
        if (Input.GetKeyDown(specialAttackButton)) controlInfo.specialAttack = true;
        if (Input.GetKeyDown(tagButton)) controlInfo.tag = true;

        return (controlInfo.move != Vector2.zero) || controlInfo.jump || controlInfo.assistAttack 
            || controlInfo.specialAttack || controlInfo.tag;
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
        // Contorl player character according to control info
        GamePlayManager.Instance.ControlPlayerCharacter(controlInfo);

        // Reset control info
        controlInfo.Reset();

        // Terminate pause event
        InputEventManager.Instance.TerminateInputEvent(this);
    }
}
