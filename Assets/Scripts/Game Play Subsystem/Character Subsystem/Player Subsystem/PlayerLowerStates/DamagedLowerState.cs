using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedLowerState : MonoBehaviour, IPlayerLowerState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody2D playerRigid;
    private const float STERN_TIME = 0.4f;
    private const float KNOCK_BACK_SPEED = 15f;
    private float sternedTime;

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerRigid = playerTransform.GetComponent<Rigidbody2D>();
        playerController.AddLowerState(PLAYER_LOWER_STATE.DAMAGED, this);
    }

    public PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.DAMAGED; }
    public bool DisableUpperBody() { return true; }

    public void StartState()
    {
        sternedTime = 0;

        KnockBack();
    }

    public void UpdateState()
    {
        sternedTime += Time.deltaTime;
        if(sternedTime > STERN_TIME)
        {
            if (playerController.StandingGround)
            {
                playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);
                playerRigid.velocity = Vector2.zero;
            }
        }
    }

    public void EndState()
    {

    }

    private void KnockBack()
    {
        Vector3 attackerPos = playerController.RecentDamagedInfo.attacker.transform.position;
        int direction = playerController.transform.position.x > attackerPos.x ? 1 : -1;

        playerRigid.velocity = new Vector2(direction * KNOCK_BACK_SPEED, KNOCK_BACK_SPEED);
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
