using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleLowerState : MonoBehaviour, IPlayerLowerState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody2D playerRigid;

    // Start is called before the first frame update

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerRigid = playerTransform.GetComponent<Rigidbody2D>();
        playerController.AddLowerState(PLAYER_LOWER_STATE.IDLE, this);
    }

    public PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.IDLE; }
    public bool DisableUpperBody() { return false; }

    public void StartState()
    {
        playerRigid.velocity = Vector2.zero;
    }

    public void UpdateState()
    {

    }

    public void EndState()
    {

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

    public void Move(int move)
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.RUNNING);
    }

    public void Damaged()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.DAMAGED);
    }

    public void Tag()
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.TAGGING);
    }

    public void Climb(bool climb)
    {
        if (climb) playerController.ChangeLowerState(PLAYER_LOWER_STATE.CLIMBING);
    }

    public void OnGround() { return; }

    public void Stop() { return; }
    public void Push(bool push) {return;}
    public void UpDown(int upDown) {return;}
}
