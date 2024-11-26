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
        playerController.AddLowerState(PLAYER_LOWER_STATE.AIMING, this);
    }

    public PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.AIMING; }
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
    public void Aim(bool isAiming) { if(!isAiming) playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);}
    public void OnAir() { playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING); }
    public void Damaged() { playerController.ChangeLowerState(PLAYER_LOWER_STATE.DAMAGED); }
    
    public void Move(int move) { return; }
    public void Climb(bool climb) { return; }
    public void Jump() {return;}
    public void Tag() { return; }
    public void OnGround() { return; }
    public void Stop() { return; }
    public void Push(bool push) {return;}
    public void UpDown(int upDown) {return;}
}
