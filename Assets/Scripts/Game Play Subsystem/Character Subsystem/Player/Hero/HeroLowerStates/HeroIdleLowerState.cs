using UnityEngine;

public class HeroIdleLowerState : PlayerLowerState<HeroLowerState>
{

    public override HeroLowerState  StateType               => HeroLowerState.Idle;
    public override bool            ShouldDisableUpperBody  => false; 

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector2.zero);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    public override void Jump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed));
        StateController.ChangeState(HeroLowerState.Jumping);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroLowerState.Jumping);
    }

    public override void Aim(bool isAiming)
    {
        if (false == isAiming) return;

        StateController.ChangeState(HeroLowerState.Aiming);
    }

    public override void Move(int move)
    {
        StateController.ChangeState(HeroLowerState.Running);
    }

    public override void Tag()
    {
        StateController.ChangeState(HeroLowerState.Tagging);
    }

    public override void Climb(bool climb)
    {
        if (false == climb) return;

        StateController.ChangeState(HeroLowerState.Climbing);
    }
}
