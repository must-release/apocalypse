using CharacterEums;
using UnityEngine;

public class TaggingLowerState : PlayerLowerStateBase
{
    private float taggingTime;
    private float time;
    private float popping;
    private bool isOnAir;

    protected override void Start()
    {
        base.Start();
        
        OwnerController.RegisterLowerState(PlayerLowerState.Tagging, this);

        taggingTime = 0.2f;
        time = 0f;
        popping = 3f;
        isOnAir = false;
    }

    public override PlayerLowerState GetStateType() { return PlayerLowerState.Tagging; }

    public override bool DisableUpperBody() { return true; }

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
                OwnerController.ChangeLowerState(PlayerLowerState.Jumping);
            else
                OwnerController.ChangeLowerState(PlayerLowerState.Idle);
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
