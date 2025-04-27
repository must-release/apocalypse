using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleLowerState : PlayerLowerStateBase
{

    public override CommonPlayerLowerState GetStateType() { return CommonPlayerLowerState.Idle; }

    public override bool ShouldDisableUpperBody() { return false; }

    public override void OnEnter()
    {
        OwnerRigid.velocity = Vector2.zero;

        OwnerController.CurrentAnimator.PlayIdle();
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    public override void Jump()
    {
        OwnerRigid.velocity = new Vector2(OwnerRigid.velocity.x, OwnerController.JumpingSpeed);
        OwnerController.ChangeLowerState(CommonPlayerLowerState.Jumping);
    }

    public override void OnAir()
    {
        OwnerController.ChangeLowerState(CommonPlayerLowerState.Jumping);
    }

    public override void Aim(bool isAiming)
    {
        if (isAiming)
            OwnerController.ChangeLowerState(CommonPlayerLowerState.Aiming);
    }

    public override void Move(int move)
    {
        OwnerController.ChangeLowerState(CommonPlayerLowerState.Running);
    }

    public override void Tag()
    {
        OwnerController.ChangeLowerState(CommonPlayerLowerState.Tagging);
    }

    public override void Climb(bool climb)
    {
        if (climb) OwnerController.ChangeLowerState(CommonPlayerLowerState.Climbing);
    }

    public override void OnGround() { return; }

    public override void Stop() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
