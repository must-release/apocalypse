using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLowerState : MonoBehaviour, IPlayerLowerState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody2D playerRigid;
    private GameObject ladder;
    private int movingDirection;

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerRigid = playerTransform.GetComponent<Rigidbody2D>();
        //ladder = playerController.nearByLadder;
        playerController.AddLowerState(CHARACTER_LOWER_STATE.CLIMBING, this);
    }

    public CHARACTER_LOWER_STATE GetState() { return CHARACTER_LOWER_STATE.CLIMBING; }

    public void StartState()
    {
        // Move Player to the ladder and remove gravity
        // playerTransform.position = new Vector3(
        //     ladder.transform.position.x, playerTransform.position.y + 1, playerTransform.position.z);
        // playerRigid.gravityScale = 0;
    }

    public void UpdateState()
    {

    }

    public void EndState()
    {

    }

    public void Move(int move)
    {
        return;
    }

    public void Stop()
    {
        return;
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
