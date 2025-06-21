using UnityEngine;

public class HeroineAttackingLowerState : HeroineLowerStateBase
{
    public override HeroineLowerState StateType => HeroineLowerState.Attacking;
    public override bool ShouldDisableUpperBody => true;

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(new Vector2(0, PlayerInfo.CurrentVelocity.y));

        StateAnimator.Play(AnimatorState.Heroine.GetHash(StateType));
        StateAnimator.Update(0.0f);
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

    public override void OnExit(HeroineLowerState _)
    {

    }

    public override void UpDown(int upDown)
    {
        _isLookingUp = upDown > 0;
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerState.Damaged);
    }

    // Called by animation event in HeroineAttackingLowerState.anim
    public void ThrowGranade()
    {
        if (_isLookingUp)
            PlayerWeapon.SetWeaponPivotRotation(70);

        PlayerWeapon.Attack();
        PlayerWeapon.SetWeaponPivotRotation(0);
    }


    /****** Private Members ******/

    private bool _isLookingUp;
}
