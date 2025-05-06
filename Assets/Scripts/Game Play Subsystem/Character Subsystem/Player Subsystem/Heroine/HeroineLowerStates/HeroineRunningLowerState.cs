using UnityEngine;

public class HeroineRunningLowerState : PlayerLowerStateBase<HeroineLowerState>
{
    /****** Public Members ******/

    public override HeroineLowerState   StateType               => HeroineLowerState.Running;
    public override bool                ShouldDisableUpperBody  => false;

    public override void OnEnter()
    {
        LowerAnimator.Play(AnimatorState.HeroineLower.Running);
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
        StateController.ChangeState(HeroineLowerState.Idle);
    }

    public override void Jump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed));
        StateController.ChangeState(HeroineLowerState.Jumping);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroineLowerState.Jumping);
    }

    public override void Aim(bool isAiming)
    {
        if (false == isAiming) return;

        StateController.ChangeState(HeroineLowerState.Aiming);
    }

    public override void Tag()
    {
        StateController.ChangeState(HeroineLowerState.Tagging);
    }

    public override void Climb(bool climb) 
    {
        if (false == climb) return;

        StateController.ChangeState(HeroineLowerState.Climbing);
    }

    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineLower.Running;
}
