using NUnit.Framework;
using UnityEngine;

public class HeroAttackingUpperState : PlayerUpperState
{
    /****** Public Members ******/

    public override UpperStateType CurrentState => HeroUpperStateType.Attacking;

    public override void InitializeState(PlayerAvatarType owningAvatar
                                        , IStateController<UpperStateType> stateController
                                        , IObjectInteractor objectInteractor
                                        , IMotionController playerMotion
                                        , ICharacterInfo playerInfo
                                        , Animator stateAnimator
                                        , PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(owningAvatar, stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);

        Debug.Assert(PlayerAvatarType.Hero == owningAvatar, $"State {CurrentState} can only be used by Hero avatar.");
        Debug.Assert(StateAnimator.HasState(0, _AttackingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
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

        var nextState = _shouldContinueAttack ? HeroUpperStateType.Attacking : HeroUpperStateType.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Attack()
    {
        _shouldContinueAttack = true;
    }

    public override void LookUp(bool lookUp)
    {
        if (false == lookUp) 
            return;

        StateController.ChangeState(UpperStateType.LookingUp);
    }

    public override void Disable()
    {
        StateController.ChangeState(UpperStateType.Disabled);
    }


    /****** Private Members ******/

    private readonly int _AttackingStateHash = AnimatorState.GetHash(PlayerAvatarType.Hero, HeroUpperStateType.Attacking);

    private float   _attackCoolTime;
    private bool    _shouldContinueAttack;
}
