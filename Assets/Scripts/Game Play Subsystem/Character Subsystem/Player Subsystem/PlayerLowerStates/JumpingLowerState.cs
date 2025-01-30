using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingLowerState : PlayerLowerStateBase
{
    private int movingDirection;

    protected override void StartLowerState()
    {
        playerController.AddLowerState(PLAYER_LOWER_STATE.JUMPING, this);
    }

    public override PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.JUMPING; }
    public override bool DisableUpperBody() { return false; }

    public override void OnEnter()
    {
        if ( 0 < playerTransform.localScale.x ) 
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
        playerRigid.velocity = new Vector2(move * playerController.MovingSpeed, playerRigid.velocity.y);

        // Set direction
        if (move != movingDirection)
        {
            playerTransform.localScale = new Vector3(-playerTransform.localScale.x,
                playerTransform.localScale.y, playerTransform.localScale.z);
            movingDirection = move;
        }
    }

    public override void Stop()
    {
        playerRigid.velocity = new Vector2(0, playerRigid.velocity.y);
    }

    public override void OnGround()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);
    }

    public override void Climb(bool climb) 
    {
        if ( climb ) 
            playerController.ChangeLowerState(PLAYER_LOWER_STATE.CLIMBING);
    }

    public override void Aim(bool isAiming) { return; }
    public override void Tag() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
