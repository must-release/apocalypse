using UnityEngine;

public class HeroineAimingLowerState : PlayerLowerStateBase<HeroineLowerState>
{
    public override HeroineLowerState   StateType               => HeroineLowerState.Aiming;
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
            StateController.ChangeState(HeroineLowerState.Idle);
    }

    public override void OnAir() 
    {
        StateController.ChangeState(HeroineLowerState.Jumping); 
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerState.Damaged); 
    }
}
