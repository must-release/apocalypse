using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLowerState : PlayerLowerStateBase
{
    private float climbingSpeed;
    private float climbUpHeight;
    private float climbDownHeight;
    
    protected override void StartLowerState()
    {
        playerController.AddLowerState(PLAYER_LOWER_STATE.CLIMBING, this);

        climbingSpeed = playerController.MovingSpeed;
        climbUpHeight = 0.1f;
        climbDownHeight = playerController.CharacterHeight / 2 + 0.2f;
    }

    public override PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.CLIMBING; }

    public override bool DisableUpperBody() { return true; }

    public override void OnEnter()
    {
        playerRigid.gravityScale = 0;

        MoveNearToClimbingObject();
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {
        playerRigid.gravityScale = playerController.Gravity;
    }


    public override void UpDown(int upDown)
    {
        playerRigid.velocity =  Vector2.up * upDown * climbingSpeed;
    }
    
    public override void Climb(bool climb)
    {
        if(!climb)
        {
            if(playerRigid.velocity.y > 0)
            {
                // Move player on the upside of the ladder
                playerTransform.position += Vector3.up * playerController.CharacterHeight / 2;
                playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);
            }
            else
            {
                // Climbed down the climing object
                if(playerController.StandingGround)
                    playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);
                else
                    playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING);
            }
        }
    }

    public override void Jump()
    {
        // jump player
        playerRigid.velocity = new Vector2(playerRigid.velocity.x, playerController.JumpingSpeed/3);

        playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING);
    }

    // Move character near the climbing object
    private void MoveNearToClimbingObject()
    {
        Transform climbingObject = playerController.CurrentControlInfo.climbingObject.transform;

        // Climbing up
        if(playerController.CurrentControlInfo.upDown > 0)
        {
            playerTransform.position = new Vector3(climbingObject.position.x,
                playerTransform.position.y + climbUpHeight, playerTransform.position.z);
        }
        // Climbing down
        else
        {
            playerTransform.position = new Vector3(climbingObject.position.x,
                playerTransform.position.y - climbDownHeight, playerTransform.position.z);
        }
    }


    /***** Inavailable State Change *****/
    public override void Move(int move) { return; }
    public override void Stop() { return; }
    public override void Aim(bool isAiming) { return; }
    public override void OnAir() { return; }
    public override void OnGround(){ return; }
    public override void Tag() { return; }
    public override void Push(bool push) { return; }
}
