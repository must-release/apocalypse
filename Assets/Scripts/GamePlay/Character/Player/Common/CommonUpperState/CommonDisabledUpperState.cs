using UnityEngine;
using UnityEngine.Assertions;

public class CommonDisabledUpperState : PlayerUpperState
{
    /****** Public Members ******/

    public override UpperStateType CurrentState => UpperStateType.Disabled;

    public override void InitializeState(PlayerAvatarType owningAvatar
                                , IStateController<UpperStateType> stateController
                                , IObjectInteractor objectInteractor
                                , IMotionController playerMotion
                                , ICharacterInfo playerInfo
                                , Animator stateAnimator
                                , PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(owningAvatar, stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);

        _DisabledStateHash = AnimatorState.GetHash(OwningAvatar, CurrentState);
        Debug.Assert(StateAnimator.HasState(0, _DisabledStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_DisabledStateHash);
    }

    public override void Enable()
    {
        StateController.ChangeState(UpperStateType.Idle);
    }


    /****** Private Members ******/

    private int _DisabledStateHash;
}
