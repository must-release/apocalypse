using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningLowerState : PlayerLowerStateBase
{
    private int movingDirection;

    protected override void StartLowerState()
    {
        playerController.AddLowerState(PLAYER_LOWER_STATE.RUNNING, this);
    }

    public override PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.RUNNING; }

    public override bool DisableUpperBody() { return false; }

    public override void OnEnter()
    {
        if ( 0 < playerTransform.localScale.x ) 
            movingDirection = 1;
        else 
            movingDirection = -1;

        playerController.LowerAnimator.SetBool("Move", true);
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
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);
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
        if ( isAiming )
            playerController.ChangeLowerState(PLAYER_LOWER_STATE.AIMING);
    }

    public override void Tag()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.TAGGING);
    }

    public override void Climb(bool climb) 
    {
        if ( climb ) 
            playerController.ChangeLowerState(PLAYER_LOWER_STATE.CLIMBING);
    }

    public override void OnGround() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
