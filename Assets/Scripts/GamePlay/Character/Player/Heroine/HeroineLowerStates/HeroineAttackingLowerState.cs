using UnityEngine;

public class HeroineAttackingLowerState : HeroineLowerState
{
    public override HeroineLowerStateType StateType => HeroineLowerStateType.Attacking;
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
            var nextState = PlayerInfo.StandingGround == null ? HeroineLowerStateType.Jumping : HeroineLowerStateType.Idle;
            StateController.ChangeState(nextState);
        }
    }

    public override void OnExit(HeroineLowerStateType _)
    {

    }

    public override void UpDown(VerticalDirection verticalInput)
    {
        _isLookingUp = (VerticalDirection.Up == verticalInput);
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerStateType.Damaged);
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
