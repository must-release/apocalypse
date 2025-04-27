using CharacterEums;
using UnityEngine;

public class DisabledUpperState : PlayerUpperStateBase
{
    public override CommonPlayerUpperState GetStateType() { return CommonPlayerUpperState.Disabled; }

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(CommonPlayerUpperState _)
    {

    }
    public override void Enable()
    {
        if( OwnerController.StandingGround )
            OwnerController.ChangeUpperState(CommonPlayerUpperState.Idle);
        else
            OwnerController.ChangeUpperState(CommonPlayerUpperState.Jumping);
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
