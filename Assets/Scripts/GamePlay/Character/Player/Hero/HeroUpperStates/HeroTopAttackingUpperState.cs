using NUnit.Framework;
using UnityEngine;

public class HeroTopAttackingUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperStateType StateType => HeroUpperStateType.TopAttacking;

    public override void InitializeState(IStateController<HeroUpperStateType> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _TopAttackingStateHash), "Hero animator does not have top attacking upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_TopAttackingStateHash);
        StateAnimator.Update(0.0f);

        PlayerWeapon.SetWeaponPivotRotation(90);
        _attackCoolTime = PlayerWeapon.Attack();

    }

    public override void OnUpdate()
    {
        _attackCoolTime -= Time.deltaTime;

        if (0 < _attackCoolTime) return;

        var nextState = _shouldContinueAttack ? HeroUpperStateType.TopAttacking : HeroUpperStateType.LookingUp;
        StateController.ChangeState(nextState);
    }

    public override void OnExit(HeroUpperStateType nextState)
    {
        if (HeroUpperStateType.TopAttacking != nextState)
            PlayerWeapon.SetWeaponPivotRotation(0);
    }

    public override void Attack()
    {
        base.Attack();

        _shouldContinueAttack = true;
    }

    public override void LookUp(bool lookUp)
    {
        if (lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroUpperStateType.Disabled : HeroUpperStateType.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroUpperStateType.Disabled);
    }



    /****** Private Members ******/

    private readonly int _TopAttackingStateHash = AnimatorState.Hero.GetHash(HeroUpperStateType.TopAttacking);

    private float   _attackCoolTime;
    private bool    _shouldContinueAttack;
}
