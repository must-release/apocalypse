using CharacterEums;
using UnityEngine;

public class DisabledUpperState : PlayerUpperStateBase
{
    protected override void StartUpperState()
    {
        playerController.AddUpperState(PLAYER_UPPER_STATE.DISABLED, this);
    }

    public override PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.DISABLED; }

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(PLAYER_UPPER_STATE _)
    {

    }
    public override void Enable()
    {
        if( playerController.StandingGround )
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE);
        else
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING);
    }


    /***** Inavailable State Change *****/
    public override void Move() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void LookUp(bool lookUp) { return; }
    public override void Aim(Vector3 aim) {return;}
    public override void Attack() { return;}
    public override void Stop() { return; }
    public override void Disable() { return; }
    public override void OnGround() { return; }
}
