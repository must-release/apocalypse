using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLowerState : MonoBehaviour, IPlayerLowerState
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
        playerController.AddLowerState(CHARACTER_LOWER_STATE.IDLE, this);
    }

    public CHARACTER_LOWER_STATE GetState() { return CHARACTER_LOWER_STATE.IDLE; }

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

    public void Move(int move)
    {
        playerController.ChangeLowerState(CHARACTER_LOWER_STATE.RUNNING);
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

    public void Stop() { return; }
    public void Push(bool push) {return;}
    public void UpDown(int upDown) {return;}
    public void Hang(float hangingPos) {return;}
}
