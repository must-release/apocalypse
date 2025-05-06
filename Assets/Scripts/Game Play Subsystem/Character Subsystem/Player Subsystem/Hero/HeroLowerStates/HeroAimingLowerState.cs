using UnityEngine;

public class HeroAimingLowerState : PlayerLowerStateBase<HeroLowerState>
{
    public override HeroLowerState      StateType               => HeroLowerState.Aiming;
    public override bool                ShouldDisableUpperBody  => false;

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector3.zero);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    public override void Aim(bool isAiming) 
    { 
        if(false == isAiming) 
            StateController.ChangeState(HeroLowerState.Idle);
    }

    public override void OnAir() 
    {
        StateController.ChangeState(HeroLowerState.Jumping); 
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroLowerState.Damaged); 
    }
}
