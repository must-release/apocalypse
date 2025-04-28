using CharacterEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningLowerState : PlayerLowerStateBase
{
    private int movingDirection;

    public override CommonPlayerLowerState GetStateType() { return CommonPlayerLowerState.Running; }

    public override bool ShouldDisableUpperBody() { return false; }

    public override void OnEnter()
    {
        if ( 0 < OwnerTransform.localScale.x ) 
            movingDirection = 1;
        else 
            movingDirection = -1;

        OwnerController.CurrentAnimator.PlayRunning();
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    public override void Move(int move)
    {
        // move player
        OwnerRigid.velocity = new Vector2(move * OwnerController.MovingSpeed, OwnerRigid.velocity.y);

        // Set direction
        if (move != movingDirection)
        {
            OwnerTransform.localScale = new Vector3(-OwnerTransform.localScale.x, 
                OwnerTransform.localScale.y, OwnerTransform.localScale.z);
            movingDirection = move;
        }
    }

    public override void Stop()
    {
        OwnerController.ChangeLowerState(CommonPlayerLowerState.Idle);
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
        if ( isAiming )
            OwnerController.ChangeLowerState(CommonPlayerLowerState.Aiming);
    }

    public override void Tag()
    {
        OwnerController.ChangeLowerState(CommonPlayerLowerState.Tagging);
    }

    public override void Climb(bool climb) 
    {
        if ( climb ) 
            OwnerController.ChangeLowerState(CommonPlayerLowerState.Climbing);
    }

    public override void OnGround() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
