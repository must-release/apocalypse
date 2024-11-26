using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingLowerState : MonoBehaviour, IPlayerLowerState
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
        playerController.AddLowerState(PLAYER_LOWER_STATE.JUMPING, this);
    }

    public PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.JUMPING; }
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
        playerRigid.velocity = new Vector2(0, playerRigid.velocity.y);
    }

    public void Damaged()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.DAMAGED);
    }

    public void OnGround()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);
    }

    public void Climb(bool climb) 
    {
        if (climb) playerController.ChangeLowerState(PLAYER_LOWER_STATE.CLIMBING);
    }

    public void Aim(bool isAiming) { return; }
    public void Tag() { return; }
    public void Jump() { return; }
    public void OnAir() { return; }
    public void Push(bool push) {return;}
    public void UpDown(int upDown) {return;}
}
