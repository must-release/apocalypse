using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLowerState : MonoBehaviour, IPlayerLowerState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody2D playerRigid;
    private float climbingSpeed = 5f;

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerRigid = playerTransform.GetComponent<Rigidbody2D>();
        playerController.AddLowerState(CHARACTER_LOWER_STATE.CLIMBING, this);
    }

    public CHARACTER_LOWER_STATE GetState() { return CHARACTER_LOWER_STATE.CLIMBING; }

    public void StartState()
    {
        playerRigid.gravityScale = 0;
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
                playerTransform.position += Vector3.up * playerController.CharacterHeight / 2;
            }
            playerController.ChangeLowerState(CHARACTER_LOWER_STATE.IDLE);
        }
    }

    public void Move(int move)
    {
        return;
    }

    public void Stop()
    {
        return;
    }

    public void Jump()
    {
        // jump player
        playerRigid.velocity = new Vector2(playerRigid.velocity.x, playerController.JumpingSpeed/3);

        playerController.ChangeLowerState(CHARACTER_LOWER_STATE.JUMPING);
    }

    public void Aim(bool isAiming)
    {

    }

    public void OnAir()
    {
        
    }

    public void Tag()
    {

    }

    public void Damaged()
    {

    }

    public void OnGround()
    {
        playerController.ChangeLowerState(CHARACTER_LOWER_STATE.IDLE);
    }

    public void Push(bool push) {return;}
}
