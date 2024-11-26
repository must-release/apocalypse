using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLowerState : MonoBehaviour, IPlayerLowerState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody2D playerRigid;
    private float climbingSpeed;
    private float climbUpHeight = 0.1f;
    private float climbDownHeight;
    

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerRigid = playerTransform.GetComponent<Rigidbody2D>();
        playerController.AddLowerState(PLAYER_LOWER_STATE.CLIMBING, this);

        climbingSpeed = playerController.MovingSpeed;
        climbDownHeight = playerController.CharacterHeight / 2 + 0.2f;
    }

    public PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.CLIMBING; }
    public bool DisableUpperBody() { return true; }

    public void StartState()
    {
        playerRigid.gravityScale = 0;

        MoveNearToClimbingObject();
    }

    public void UpdateState()
    {

    }

    public void EndState()
    {
        playerRigid.gravityScale = playerController.Gravity;
    }


    public void UpDown(int upDown)
    {
        playerRigid.velocity =  Vector2.up * upDown * climbingSpeed;
    }
    
    public void Climb(bool climb)
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

    public void Jump()
    {
        // jump player
        playerRigid.velocity = new Vector2(playerRigid.velocity.x, playerController.JumpingSpeed/3);

        playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING);
    }

    public void Damaged() { playerController.ChangeLowerState(PLAYER_LOWER_STATE.DAMAGED); }

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
    public void Move(int move) { return; }
    public void Stop() { return; }
    public void Aim(bool isAiming) { return; }
    public void OnAir() { return; }
    public void OnGround(){ return; }
    public void Tag() { return; }
    public void Push(bool push) { return; }
}
