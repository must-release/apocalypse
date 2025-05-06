using UnityEngine;

public class HeroineIdleLowerState : PlayerLowerStateBase<HeroineLowerState>
{

    public override HeroineLowerState   StateType               => HeroineLowerState.Idle;
    public override bool                ShouldDisableUpperBody  => false; 

    public override void OnEnter()
    {
        LowerAnimator.Play(AnimatorState.HeroineLower.Idle);
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

    public override void Move(int move)
    {
        StateController.ChangeState(HeroineLowerState.Running);
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

    protected override string AnimationClipPath => AnimationClipAsset.HeroineLower.Idle;
}
