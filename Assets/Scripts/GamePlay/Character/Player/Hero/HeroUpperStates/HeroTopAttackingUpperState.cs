﻿using NUnit.Framework;
using UnityEngine;

public class HeroTopAttackingUpperState : PlayerUpperState
{
    /****** Public Members ******/

    public override UpperStateType CurrentState => HeroUpperStateType.TopAttacking;

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
        Debug.Assert(StateAnimator.HasState(0, _TopAttackingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
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

        var nextState = _shouldContinueAttack ? HeroUpperStateType.TopAttacking : UpperStateType.LookingUp;
        StateController.ChangeState(nextState);
    }

    public override void OnExit(UpperStateType nextState)
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

        var nextState = PlayerInfo.StandingGround == null ? UpperStateType.Disabled : UpperStateType.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Disable()
    {
        StateController.ChangeState(UpperStateType.Disabled);
    }


    /****** Private Members ******/

    private readonly int _TopAttackingStateHash = AnimatorState.GetHash(PlayerAvatarType.Hero, HeroUpperStateType.TopAttacking);

    private float   _attackCoolTime;
    private bool    _shouldContinueAttack;
}
