using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningLowerState : PlayerLowerStateBase
{
    private int movingDirection;

    public override PlayerLowerState GetStateType() { return PlayerLowerState.Running; }

    public override bool DisableUpperBody() { return false; }

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
        OwnerController.ChangeLowerState(PlayerLowerState.Idle);
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
        if ( isAiming )
            OwnerController.ChangeLowerState(PlayerLowerState.Aiming);
    }

    public override void Tag()
    {
        OwnerController.ChangeLowerState(PlayerLowerState.Tagging);
    }

    public override void Climb(bool climb) 
    {
        if ( climb ) 
            OwnerController.ChangeLowerState(PlayerLowerState.Climbing);
    }

    public override void OnGround() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
