using CharacterEums;
using UnityEngine;

public class TaggingLowerState : PlayerLowerStateBase
{
    private float taggingTime;
    private float time;
    private float popping;
    private bool isOnAir;

    protected override void StartLowerState()
    {
        playerController.AddLowerState(PLAYER_LOWER_STATE.TAGGING, this);

        taggingTime = 0.2f;
        time = 0f;
        popping = 3f;
        isOnAir = false;
    }

    public override PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.TAGGING; }

    public override bool DisableUpperBody() { return true; }

    public override void OnEnter()
    {
        // Stop player
        playerRigid.velocity = Vector2.up * popping;

        // Change player character
        playerController.ChangeCharacter(playerController.CurrentCharacter == PLAYER.HERO ? PLAYER.HEROINE : PLAYER.HERO);
    
        // Reset Time
        time = 0f;
    }

    public override void OnUpdate()
    {
        time += Time.deltaTime;
        if (time > taggingTime)
        {
            if ( isOnAir )
                playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING);
            else
                playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);
        }
    }

    public override void OnExit()
    {
        
    }

    public override void OnAir() { isOnAir = true; }

    public override void OnGround() { isOnAir = false; }


    public override void Tag() { return; }
    public override void Move(int move) { return; }
    public override void Climb(bool climb) { return; }
    public override void Jump() { return; }
    public override void Aim(bool isAiming) { return; }
    public override void Stop() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
