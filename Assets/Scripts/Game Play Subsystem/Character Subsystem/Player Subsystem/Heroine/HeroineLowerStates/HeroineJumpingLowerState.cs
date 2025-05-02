using CharacterEnums;
using UnityEngine;

public class HeroineJumpingLowerState : PlayerLowerStateBase<HeroineLowerState>
{
    /****** Public Members ******/

    public override HeroineLowerState   StateType               => HeroineLowerState.Jumping;
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
        PlayerMotion.SetVelocity(new Vector2(0, PlayerInfo.CurrentVelocity.y));
    }

    public override void OnGround()
    {
        StateController.ChangeState(HeroineLowerState.Idle);
    }

    public override void Climb(bool climb) 
    {
        if (false == climb) return;

        StateController.ChangeState(HeroineLowerState.Climbing);
    }
}
