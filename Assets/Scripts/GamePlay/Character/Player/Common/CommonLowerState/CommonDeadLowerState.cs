using UnityEngine;
using UnityEngine.Assertions;

public class CommonDeadLowerState : PlayerLowerState
{
    /****** Public Members ******/

    public override LowerStateType CurrentState => LowerStateType.Dead;
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

        _deadStateHash = AnimatorState.GetHash(owningAvatar, CurrentState);
        Debug.Assert(StateAnimator.HasState(0, AnimatorState.GetHash(owningAvatar, CurrentState)), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
    }

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector2.zero);
        StateAnimator.Play(_deadStateHash);
        StateAnimator.Update(0.0f);

        _isAnimationPlaying = true;
    }

    public override void OnUpdate()
    {
        if (false == _isAnimationPlaying) return;

        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        if (1.0f <= stateInfo.normalizedTime)
        {
            CharacterManager.Instance.ProcessPlayersDeath();
            _isAnimationPlaying = false;
        }
    }


    /****** Private Members ******/

    private int _deadStateHash;
    private bool _isAnimationPlaying;
}
