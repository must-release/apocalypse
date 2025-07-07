using NUnit.Framework;
using UnityEngine;

public class HeroineAttackingLowerState : PlayerLowerState
{
    public override LowerStateType CurrentState => HeroineLowerStateType.Attacking;
    public override bool ShouldDisableUpperBody => true;

    public override void InitializeState(PlayerAvatarType owningAvatar
                                         , IStateController<LowerStateType> stateController
                                         , IObjectInteractor objectInteractor
                                         , IMotionController playerMotion
                                         , ICharacterInfo playerInfo
                                         , Animator stateAnimator
                                         , PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(owningAvatar, stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);

        Assert.IsTrue(PlayerAvatarType.Heroine == owningAvatar, "HeroineAttackingLowerState can only be used by Heroine avatar.");
        _attackingStateHash = AnimatorState.GetHash(owningAvatar, CurrentState);
        Assert.IsTrue(StateAnimator.HasState(0, _attackingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
    }

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(new Vector2(0, PlayerInfo.CurrentVelocity.y));

        StateAnimator.Play(_attackingStateHash);
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

    public override void UpDown(VerticalDirection verticalInput)
    {
        _isLookingUp = (VerticalDirection.Up == verticalInput);
    }

    public override void OnDamaged()
    {
        StateController.ChangeState(LowerStateType.Damaged);
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
    private int _attackingStateHash;
}

