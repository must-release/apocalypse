using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningLowerState : MonoBehaviour, IPlayerLowerState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody2D playerRigid;
    private int movingDirection;

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerRigid = playerTransform.GetComponent<Rigidbody2D>();
        playerController.AddLowerState(PLAYER_LOWER_STATE.RUNNING, this);
    }

    public PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.RUNNING; }
    public bool DisableUpperBody() { return false; }

    public void StartState()
    {
        if (playerTransform.localScale.x > 0) movingDirection = 1;
        else movingDirection = -1;
    }

    public void UpdateState()
    {

    }

    public void EndState()
    {

    }

    public void Move(int move)
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

    public void Stop()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);
    }

    public void Jump()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x, playerController.JumpingSpeed);
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING);
    }

    public void OnAir()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING);
    }

    public void Aim(bool isAiming)
    {
        if (isAiming)
        {
            playerController.ChangeLowerState(PLAYER_LOWER_STATE.AIMING);
        }
    }

    public void Tag()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.TAGGING);
    }

    public void Damaged()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.DAMAGED);
    }

    public void Climb(bool climb) 
    {
        if (climb) playerController.ChangeLowerState(PLAYER_LOWER_STATE.CLIMBING);
    }

    public void OnGround() { return; }
    public void Push(bool push) {return;}
    public void UpDown(int upDown) {return;}
}
