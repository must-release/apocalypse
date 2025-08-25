using UnityEngine;
using UnityEngine.Assertions;

public class HeroineAimAttackingLowerState : PlayerLowerState
{
    public override LowerStateType CurrentState => HeroineLowerStateType.AimAttacking;
    public override bool ShouldDisableUpperBody => true;

    public override void InitializeState(PlayerAvatarType owningAvatar
                                     , IStateController<LowerStateType> stateController
                                     , IObjectInteractor objectInteractor
                                     , IMotionController playerMotion
                                     , ICharacterInfo playerInfo
                                     , Animator stateAnimator
                                     , PlayerWeaponBase playerWeapon
                                     , ControlInputBuffer inputBuffer)
    {
        base.InitializeState(owningAvatar, stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon, inputBuffer);

        Debug.Assert(PlayerAvatarType.Heroine == owningAvatar, "HeroineAttackingLowerState can only be used by Heroine avatar.");
        _aimAttackingStateHash = AnimatorState.GetHash(owningAvatar, CurrentState);
        Debug.Assert(StateAnimator.HasState(0, _aimAttackingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_aimAttackingStateHash);
        
        postDelay = PlayerWeapon.Attack();
        passedTime = 0f;
    }

    public override void OnUpdate()
    {
        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        passedTime += Time.deltaTime;

        if (1.0f <= stateInfo.normalizedTime && postDelay < passedTime)
        {
            var nextState = PlayerInfo.StandingGround == null ? LowerStateType.Jumping : LowerStateType.Idle;
            StateController.ChangeState(nextState);
        }
    }

    public override void OnExit(LowerStateType _)
    {
        PlayerWeapon.SetWeaponPivotRotation(0);
    }

    public override void OnDamaged()
    {
        StateController.ChangeState(LowerStateType.Damaged);
    }


    /****** Private Members ******/

    private float postDelay;
    private float passedTime;
    private int _aimAttackingStateHash;
}
