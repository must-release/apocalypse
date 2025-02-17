using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleLowerState : PlayerLowerStateBase
{
    protected override void StartLowerState()
    {
        playerController.AddLowerState(PLAYER_LOWER_STATE.IDLE, this);
    }

    public override PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.IDLE; }

    public override bool DisableUpperBody() { return false; }

    public override void OnEnter()
    {
        playerRigid.velocity = Vector2.zero;

        playerController.LowerAnimator.SetBool("Move", false);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    public override void Jump()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x, playerController.JumpingSpeed);
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING);
    }

    public override void OnAir()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING);
    }

    public override void Aim(bool isAiming)
    {
        if (isAiming)
            playerController.ChangeLowerState(PLAYER_LOWER_STATE.AIMING);
    }

    public override void Move(int move)
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.RUNNING);
    }

    public override void Tag()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.TAGGING);
    }

    public override void Climb(bool climb)
    {
        if (climb) playerController.ChangeLowerState(PLAYER_LOWER_STATE.CLIMBING);
    }

    public override void OnGround() { return; }

    public override void Stop() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
