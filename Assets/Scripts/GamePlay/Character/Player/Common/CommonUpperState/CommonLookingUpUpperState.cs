using UnityEngine;
using UnityEngine.Assertions;

public class CommonLookingUpUpperState : PlayerUpperState
{
    /****** Public Members ******/

    public override UpperStateType CurrentState => UpperStateType.LookingUp;

    public override void InitializeState(PlayerAvatarType owningAvatar
                                        , IStateController<UpperStateType> stateController
                                        , IObjectInteractor objectInteractor
                                        , IMotionController playerMotion
                                        , ICharacterInfo playerInfo
                                        , Animator stateAnimator
                                        , PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(owningAvatar, stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);

        _IdleLookingUpStateHash     = AnimatorState.GetHash(owningAvatar, UpperStateType.Idle, "LookingUp");
        _RunningLookingUpStateHash  = AnimatorState.GetHash(owningAvatar, UpperStateType.Running, "LookingUp");

        Debug.Assert(StateAnimator.HasState(0, _IdleLookingUpStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
        Debug.Assert(StateAnimator.HasState(0, _RunningLookingUpStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
    }

    public override void OnEnter()
    {
        var nextStateHash = PlayerInfo.IsMoving ? _RunningLookingUpStateHash : _IdleLookingUpStateHash;
        StateAnimator.Play(nextStateHash, 0, LowerStateInfo.AnimationNormalizedTime);
        StateAnimator.Update(0.0f);
    }

    public override void LookUp(bool lookUp) 
    { 
        if(lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? UpperStateType.Disabled : UpperStateType.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        if (HorizontalDirection.None == horizontalInput)
        {
            if (_RunningLookingUpStateHash == stateInfo.shortNameHash)
            {
                StateAnimator.Play(_IdleLookingUpStateHash, 0, LowerStateInfo.AnimationNormalizedTime);
                StateAnimator.Update(0.0f);
            }
        }
        else
        {
            if (_IdleLookingUpStateHash == stateInfo.shortNameHash)
            {
                StateAnimator.Play(_RunningLookingUpStateHash, 0, LowerStateInfo.AnimationNormalizedTime);
                StateAnimator.Update(0.0f);
            }
        }

    }

    public override void Disable()
    {
        StateController.ChangeState(UpperStateType.Disabled);
    }


    /****** Private Members ******/

    private int _IdleLookingUpStateHash;
    private int _RunningLookingUpStateHash;
}
