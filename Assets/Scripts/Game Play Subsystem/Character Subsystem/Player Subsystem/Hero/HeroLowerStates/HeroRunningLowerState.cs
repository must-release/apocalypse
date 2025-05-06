using UnityEngine;

public class HeroRunningLowerState : PlayerLowerStateBase<HeroLowerState>
{
    /****** Public Members ******/

    public override HeroLowerState   StateType               => HeroLowerState.Running;
    public override bool                ShouldDisableUpperBody  => false;

    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    public override void Move(int move)
    {
        if (0 == move) return;

        // move player
        PlayerMotion.SetVelocity(new Vector2(move * PlayerInfo.MovingSpeed, PlayerInfo.CurrentVelocity.y));

        // set direction
        FacingDirection direction = move < 0 ? FacingDirection.Left : FacingDirection.Right;
        if (direction != PlayerInfo.FacingDirection)
        {
            PlayerMotion.SetFacingDirection(direction);
        }
    }

    public override void Stop()
    {
        StateController.ChangeState(HeroLowerState.Idle);
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
