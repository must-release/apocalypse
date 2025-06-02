using UnityEngine;

public class HeroineRunningLowerState : HeroineLowerStateBase
{
    /****** Public Members ******/

    public override HeroineLowerState   StateType               => HeroineLowerState.Running;
    public override bool                ShouldDisableUpperBody  => false;

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.HeroineLower.Running);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(HeroineLowerState _)
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
        StateController.ChangeState(HeroineLowerState.Idle);
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerState.Attacking);
    }

    public override void StartJump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed));
        StateController.ChangeState(HeroineLowerState.Jumping);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroineLowerState.Jumping);
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim) return;

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

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerState.Damaged);
    }

    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineLower.Running;
}
