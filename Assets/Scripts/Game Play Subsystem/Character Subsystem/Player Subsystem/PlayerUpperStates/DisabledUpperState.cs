using CharacterEums;
using UnityEngine;

public class DisabledUpperState : PlayerUpperStateBase
{
    protected override void StartUpperState()
    {
        playerController.RegisterUpperState(PlayerUpperState.Disabled, this);
    }

    public override PlayerUpperState GetStateType() { return PlayerUpperState.Disabled; }

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(PlayerUpperState _)
    {

    }
    public override void Enable()
    {
        if( playerController.StandingGround )
            playerController.ChangeUpperState(PlayerUpperState.Idle);
        else
            playerController.ChangeUpperState(PlayerUpperState.Jumping);
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
