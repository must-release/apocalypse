using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadLowerState : MonoBehaviour, IPlayerLowerState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody2D playerRigid;
    private const float ANIMATION_PLAYTIME = 0.5f;
    private float animationTime;

    public void Start()
    {
        playerTransform     =   transform.parent.parent;
        playerController    =   playerTransform.GetComponent<PlayerController>();
        playerRigid         =   playerTransform.GetComponent<Rigidbody2D>();

        playerController.AddLowerState(PLAYER_LOWER_STATE.DEAD, this);
    }

    public PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.DEAD; }
    public bool DisableUpperBody() { return true; }

    public void StartState()
    {
        animationTime = 0;
    }

    public void UpdateState()
    {
        animationTime += Time.deltaTime;
        if( animationTime < ANIMATION_PLAYTIME )
            return;
    }

    public void EndState()
    {

    }

    public void Jump() { return; }
    public void OnAir() { }
    public void OnGround() { } 
    public void Aim(bool isAiming) { return; }
    public void Move(int move) { return; }
    public void Damaged() { return; }
    public void Tag() { return; }
    public void Climb(bool climb) { return; }
    public void Stop() { return; }
    public void Push(bool push) {return;}
    public void UpDown(int upDown) {return;}
}
