using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingLowerState : PlayerLowerStateBase
{
    private int movingDirection;

    protected override void Start()
    {
        base.Start();

        OwnerController.RegisterLowerState(PlayerLowerState.Jumping, this);
    }

    public override PlayerLowerState GetStateType() { return PlayerLowerState.Jumping; }
    public override bool DisableUpperBody() { return false; }

    public override void OnEnter()
    {
        if ( 0 < OwnerTransform.localScale.x ) 
            movingDirection = 1;
        else 
            movingDirection = -1;
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
        if ( move * movingDirection < 0)
        {
            OwnerTransform.localScale = new Vector3(-OwnerTransform.localScale.x,
                OwnerTransform.localScale.y, OwnerTransform.localScale.z);
            movingDirection = move;
        }
    }

    public override void Stop()
    {
        OwnerRigid.velocity = new Vector2(0, OwnerRigid.velocity.y);
    }

    public override void OnGround()
    {
        OwnerController.ChangeLowerState(PlayerLowerState.Idle);
    }

    public override void Climb(bool climb) 
    {
        if ( climb ) 
            OwnerController.ChangeLowerState(PlayerLowerState.Climbing);
    }

    public override void Aim(bool isAiming) { return; }
    public override void Tag() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
