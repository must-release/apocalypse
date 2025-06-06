using UnityEngine;

public class HeroJumpingLowerState : PlayerLowerState<HeroLowerState>
{
    /****** Public Members ******/

    public override HeroLowerState  StateType               => HeroLowerState.Jumping;
    public override bool            ShouldDisableUpperBody  => false;

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
        if (direction != PlayerInfo.CurrentFacingDirection)
        {
            PlayerMotion.SetFacingDirection(direction);
        }
    }

    public override void Stop()
    {
        PlayerMotion.SetVelocity(new Vector2(0, PlayerInfo.CurrentVelocity.y));
    }

    public override void OnGround()
    {
        StateController.ChangeState(HeroLowerState.Idle);
    }

    public override void Climb(bool climb) 
    {
        if (false == climb) return;

        StateController.ChangeState(HeroLowerState.Climbing);
    }
}
