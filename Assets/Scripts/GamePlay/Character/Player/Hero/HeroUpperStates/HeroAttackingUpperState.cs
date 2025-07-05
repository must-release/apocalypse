using NUnit.Framework;
using UnityEngine;

public class HeroAttackingUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.Attacking;

    public override void InitializeState(IStateController<HeroUpperState> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _AttackingStateHash), "Hero animator does not have attacking upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_AttackingStateHash);
        StateAnimator.Update(0.0f);

        _attackCoolTime         = PlayerWeapon.Attack();
        _shouldContinueAttack   = false;
    }

    public override void OnUpdate()
    {
        _attackCoolTime -= Time.deltaTime;

        if (0 < _attackCoolTime) return;

        var nextState = _shouldContinueAttack ? HeroUpperState.Attacking : HeroUpperState.Idle;
        StateController.ChangeState(nextState);
    }

    public override void OnExit(HeroUpperState _)
    {

    }

    public override void Attack()
    {
        base.Attack();

        _shouldContinueAttack = true;
    }

    public override void LookUp(bool lookUp)
    {
        if (false == lookUp) return;

        StateController.ChangeState(HeroUpperState.LookingUp);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroUpperState.Disabled);
    }



    /****** Private Members ******/

    private readonly int _AttackingStateHash = AnimatorState.Hero.GetHash(HeroUpperState.Attacking);

    private float   _attackCoolTime;
    private bool    _shouldContinueAttack;
}
