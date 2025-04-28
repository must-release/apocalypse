using CharacterEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLowerState : PlayerLowerStateBase
{
    private float climbingSpeed;
    private float climbUpHeight;
    private float climbDownHeight;
    
    public override void SetOwner(PlayerController playerController)
    {
        base.SetOwner(playerController);

        climbingSpeed = OwnerController.MovingSpeed;
        climbUpHeight = 0.1f;
        climbDownHeight = OwnerController.CharacterHeight / 2 + 0.2f;
    }

    public override CommonPlayerLowerState GetStateType() { return CommonPlayerLowerState.Climbing; }

    public override bool ShouldDisableUpperBody() { return true; }

    public override void OnEnter()
    {
        OwnerRigid.gravityScale = 0;

        MoveNearToClimbingObject();
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {
        OwnerRigid.gravityScale = OwnerController.Gravity;
    }


    public override void UpDown(int upDown)
    {
        OwnerRigid.velocity =  Vector2.up * upDown * climbingSpeed;
    }
    
    public override void Climb(bool climb)
    {
        if(!climb)
        {
            if(OwnerRigid.velocity.y > 0)
            {
                // Move player on the upside of the ladder
                OwnerTransform.position += Vector3.up * OwnerController.CharacterHeight / 2;
                OwnerController.ChangeLowerState(CommonPlayerLowerState.Idle);
            }
            else
            {
                // Climbed down the climing object
                if(OwnerController.StandingGround)
                    OwnerController.ChangeLowerState(CommonPlayerLowerState.Idle);
                else
                    OwnerController.ChangeLowerState(CommonPlayerLowerState.Jumping);
            }
        }
    }

    public override void Jump()
    {
        // jump player
        OwnerRigid.velocity = new Vector2(OwnerRigid.velocity.x, OwnerController.JumpingSpeed/3);

        OwnerController.ChangeLowerState(CommonPlayerLowerState.Jumping);
    }

    // Move character near the climbing object
    private void MoveNearToClimbingObject()
    {
        Transform climbingObject = OwnerController.CurrentControlInfo.climbingObject.transform;

        // Climbing up
        if(OwnerController.CurrentControlInfo.upDown > 0)
        {
            OwnerTransform.position = new Vector3(climbingObject.position.x,
                OwnerTransform.position.y + climbUpHeight, OwnerTransform.position.z);
        }
        // Climbing down
        else
        {
            OwnerTransform.position = new Vector3(climbingObject.position.x,
                OwnerTransform.position.y - climbDownHeight, OwnerTransform.position.z);
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
