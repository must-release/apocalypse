using UnityEngine;

public class HeroineAimAttackingLowerState : HeroineLowerState
{
    public override HeroineLowerStateType StateType => HeroineLowerStateType.AimAttacking;
    public override bool ShouldDisableUpperBody => true;

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.Heroine.GetHash(StateType));
        
        postDelay = PlayerWeapon.Attack();
        passedTime = 0f;
    }

    public override void OnUpdate()
    {
        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        passedTime += Time.deltaTime;

        if (1.0f <= stateInfo.normalizedTime && postDelay < passedTime)
        {
            var nextState = PlayerInfo.StandingGround == null ? HeroineLowerStateType.Jumping : HeroineLowerStateType.Idle;
            StateController.ChangeState(nextState);
        }
    }

    public override void OnExit(HeroineLowerStateType _)
    {
        PlayerWeapon.SetWeaponPivotRotation(0);
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerStateType.Damaged);
    }


    /****** Private Members ******/

    private float postDelay     = 0f;
    private float passedTime    = 0f;
}
