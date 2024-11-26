using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggingLowerState : MonoBehaviour, IPlayerLowerState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody2D playerRigid;
    private float taggingTime;
    private float time;
    private float popping;
    private bool isOnAir;

    // Start is called before the first frame update

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerRigid = playerTransform.GetComponent<Rigidbody2D>();
        playerController.AddLowerState(PLAYER_LOWER_STATE.TAGGING, this);
        taggingTime = 0.2f;
        time = 0f;
        popping = 3f;
        isOnAir = false;
    }

    public PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.TAGGING; }
    public bool DisableUpperBody() { return true; }

    public void StartState()
    {
        // Stop player
        playerRigid.velocity = Vector2.up * popping;

        // Change player character
        playerController.ChangeCharacter(playerController.CurrentCharacter == PLAYER.HERO ? PLAYER.HEROINE : PLAYER.HERO);
    
        // Reset Time
        time = 0f;
    }

    public void UpdateState()
    {
        time += Time.deltaTime;
        if (time > taggingTime)
        {
            if (isOnAir)
            {
                playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING);
            }
            else
            {
                playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);
            }
        }
    }

    public void EndState()
    {
        
    }

    public void OnAir() { isOnAir = true; }

    public void OnGround() { isOnAir = false; }

    public void Damaged() { playerController.ChangeLowerState(PLAYER_LOWER_STATE.DAMAGED); }


    public void Tag() { return; }
    public void Move(int move) { return; }
    public void Climb(bool climb) { return; }
    public void Jump() { return; }
    public void Aim(bool isAiming) { return; }
    public void Stop() { return; }
    public void Push(bool push) {return;}
    public void UpDown(int upDown) {return;}
    public void Climb() {return;}
}
