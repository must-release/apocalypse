using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleLowerState : PlayerLowerStateBase
{

    public override PlayerLowerState GetStateType() { return PlayerLowerState.Idle; }

    public override bool DisableUpperBody() { return false; }

    public override void OnEnter()
    {
        OwnerRigid.velocity = Vector2.zero;

        OwnerController.LowerAnimator.PlayIdle();
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
        OwnerController.ChangeLowerState(PlayerLowerState.Jumping);
    }

    public override void OnAir()
    {
        OwnerController.ChangeLowerState(PlayerLowerState.Jumping);
    }

    public override void Aim(bool isAiming)
    {
        if (isAiming)
            OwnerController.ChangeLowerState(PlayerLowerState.Aiming);
    }

    public override void Move(int move)
    {
        OwnerController.ChangeLowerState(PlayerLowerState.Running);
    }

    public override void Tag()
    {
        OwnerController.ChangeLowerState(PlayerLowerState.Tagging);
    }

    public override void Climb(bool climb)
    {
        if (climb) OwnerController.ChangeLowerState(PlayerLowerState.Climbing);
    }

    public override void OnGround() { return; }

    public override void Stop() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
