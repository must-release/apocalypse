using CharacterEnums;
using UnityEngine;

public class TaggingLowerState : PlayerLowerStateBase
{
    private float taggingTime = 0.2f;
    private float time = 0f;
    private float popping = 3f;
    private bool isOnAir = false;

    public override CommonPlayerLowerState GetStateType() { return CommonPlayerLowerState.Tagging; }

    public override bool ShouldDisableUpperBody() { return true; }

    public override void OnEnter()
    {
        // Stop player
        OwnerRigid.velocity = Vector2.up * popping;

        // Change player character
        OwnerController.ChangePlayer(OwnerController.CurrentPlayerType == PlayerType.Hero ? PlayerType.Heroine : PlayerType.Hero);
    
        // Reset Time
        time = 0f;
    }

    public override void OnUpdate()
    {
        time += Time.deltaTime;
        if (time > taggingTime)
        {
            if ( isOnAir )
                OwnerController.ChangeLowerState(CommonPlayerLowerState.Jumping);
            else
                OwnerController.ChangeLowerState(CommonPlayerLowerState.Idle);
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
