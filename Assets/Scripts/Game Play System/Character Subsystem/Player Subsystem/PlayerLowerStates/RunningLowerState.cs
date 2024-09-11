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
        playerController.AddLowerState(CHARACTER_LOWER_STATE.RUNNING, this);
    }

    public CHARACTER_LOWER_STATE GetState() { return CHARACTER_LOWER_STATE.RUNNING; }

    public void StartState()
    {
        if (playerTransform.localScale.x > 0) movingDirection = -1;
        else movingDirection = 1;
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
        playerController.ChangeLowerState(CHARACTER_LOWER_STATE.IDLE);
    }

    public void Climb(int upDown)
    {

    }

    public void Jump()
    {
        // jump player
        playerRigid.velocity = new Vector2(playerRigid.velocity.x, playerController.JumpingSpeed);
    }

    public void Aim(bool isAiming)
    {

    }

    public void OnAir()
    {
        playerController.ChangeLowerState(CHARACTER_LOWER_STATE.JUMPING);
    }

    public void Tag()
    {
        playerController.ChangeLowerState(CHARACTER_LOWER_STATE.TAGGING);
    }

    public void Damaged()
    {

    }

    public void OnGround() { return; }
    public void Push(bool push) {return;}
    public void UpDown(int upDown) {return;}
    public void Hang(float hangingPos) {return;}
}
