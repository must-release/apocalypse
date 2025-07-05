using NUnit.Framework;
using UnityEngine;

public class HeroTopAttackingUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.TopAttacking;

    public override void InitializeState(IStateController<HeroUpperState> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
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

        var nextState = _shouldContinueAttack ? HeroUpperState.TopAttacking : HeroUpperState.LookingUp;
        StateController.ChangeState(nextState);
    }

    public override void OnExit(HeroUpperState nextState)
    {
        if (HeroUpperState.TopAttacking != nextState)
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

        var nextState = PlayerInfo.StandingGround == null ? HeroUpperState.Disabled : HeroUpperState.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroUpperState.Disabled);
    }



    /****** Private Members ******/

    private readonly int _TopAttackingStateHash = AnimatorState.Hero.GetHash(HeroUpperState.TopAttacking);

    private float   _attackCoolTime;
    private bool    _shouldContinueAttack;
}
