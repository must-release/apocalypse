using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControlEvent : InputEvent, KeySettingsObserver
{
    /****** Public Members *******/

    public void Start()
    {
        SettingsManager.Instance.AddObserver(this);
    }

    public void KeySettingsUpdated()
    {
        var keySettings = SettingsManager.Instance.KeySettingInfo;

        upButton            = keySettings.upButton.buttonKeyCode;
        rightButton         = keySettings.rightButton.buttonKeyCode;
        leftButton          = keySettings.leftButton.buttonKeyCode;
        downButton          = keySettings.downButton.buttonKeyCode;
        jumpButton          = keySettings.jumpButton.buttonKeyCode;
        attackButton        = keySettings.attackButton.buttonKeyCode;
        aimButton           = keySettings.aimButton.buttonKeyCode;
        specialAttackButton = keySettings.specialAttackButton.buttonKeyCode;
        tagButton           = keySettings.tagButton.buttonKeyCode;
    }

    public override bool DetectInput()
    {
        bool buttonClicked  = SetControlInfo();
        bool aimed          = SetAimControlInfo();
        return buttonClicked || aimed;
    }
    public override bool CheckCompatibility(List<InputEvent> eventList, BaseUI baseUI, SubUI subUI)
    {
        bool isEventListEmpty = eventList.Count == 0;
        bool isValidUI = baseUI == BaseUI.Control && subUI == SubUI.None;

        return isEventListEmpty && isValidUI;
    }

    public override void PlayEvent()
    {
        // Control player character according to control info
        AD.GamePlay.GamePlayManager.Instance.ControlPlayerCharacter(controlInfo);

        // Terminate pause event
        InputEventManager.Instance.TerminateInputEvent(this);
    }

    /****** Private Members ******/

    private KeyCode
    upButton, rightButton, leftButton, downButton, jumpButton, attackButton,
    assistAttackButton, aimButton, specialAttackButton, tagButton, interactButton;

    private ControlInfo controlInfo = new ControlInfo();

    private bool SetControlInfo()
    {
        // Set Movement & Stop control
        HorizontalDirection prevHInput = controlInfo.HorizontalInput;
        controlInfo.HorizontalInput = HorizontalDirection.None;
        if (Input.GetKey(rightButton)) controlInfo.HorizontalInput = HorizontalDirection.Right;
        if (Input.GetKey(leftButton)) controlInfo.HorizontalInput = HorizontalDirection.Left;
        bool applyHInput = controlInfo.HorizontalInput != HorizontalDirection.None || prevHInput != HorizontalDirection.None;

        // Set lookUp & Climb & Stop control
        VerticalDirection prevVInput = controlInfo.VerticalInput;
        controlInfo.VerticalInput = VerticalDirection.None;
        if (Input.GetKey(upButton)) controlInfo.VerticalInput = VerticalDirection.Up;
        if (Input.GetKey(downButton)) controlInfo.VerticalInput= VerticalDirection.Down;
        bool applyVInput = controlInfo.VerticalInput != VerticalDirection.None || prevVInput != VerticalDirection.None;

        // Set jump control
        controlInfo.IsJumpStarted   = Input.GetKeyDown(jumpButton);
        controlInfo.IsJumping       = Input.GetKey(jumpButton);
        bool applyJump = controlInfo.IsJumpStarted || controlInfo.IsJumping || Input.GetKeyUp(jumpButton);

        // Set IsAttacking control
        controlInfo.IsAttacking = Input.GetKeyDown(attackButton);

        // Set IsTagging control
        controlInfo.IsTagging = Input.GetKeyDown(tagButton);

        return applyHInput || applyVInput || applyJump || controlInfo.IsAttacking || controlInfo.IsTagging;
    }

    // Set AimingPosition info according to mouse position
    private bool SetAimControlInfo()
    {
        Vector3 prevAim = controlInfo.AimingPosition;
        if (Input.GetKey(aimButton))
        {
            Vector3 mousePosition = Input.mousePosition;

            float planeZ = 0f;
            float distance = Mathf.Abs(Camera.main.transform.position.z - planeZ);

            Vector3 screenPos = new Vector3(mousePosition.x, mousePosition.y, distance);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            controlInfo.AimingPosition = worldPos;
        }
        else
        {
            controlInfo.AimingPosition = Vector3.zero;
        }
        return prevAim != Vector3.zero || controlInfo.AimingPosition != Vector3.zero;
    }
}
