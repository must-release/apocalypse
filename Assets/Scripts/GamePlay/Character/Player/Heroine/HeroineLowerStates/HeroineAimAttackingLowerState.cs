using UnityEngine;

public class HeroineAimAttackingLowerState : HeroineLowerStateBase
{
    public override HeroineLowerState StateType => HeroineLowerState.AimAttacking;
    public override bool ShouldDisableUpperBody => true;

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.HeroineLower.Attacking);
        PlayerWeapon.Attack();
    }

    public override void OnUpdate()
    {
        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        if (1.0f <= stateInfo.normalizedTime)
        {
            var nextState = PlayerInfo.StandingGround == null ? HeroineLowerState.Jumping : HeroineLowerState.Idle;
            StateController.ChangeState(nextState);
        }
    }

    public override void OnExit()
    {

    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerState.Damaged);
    }


    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineLower.AimAttacking;
}
