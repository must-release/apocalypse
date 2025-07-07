using UnityEngine;
using UnityEngine.Assertions;

public class CommonAimingLowerState : PlayerLowerState
{
    public override LowerStateType CurrentState => LowerStateType.Aiming;
    public override bool ShouldDisableUpperBody => false;

    public override void InitializeState(PlayerAvatarType owningAvatar
                                         , IStateController<LowerStateType> stateController
                                         , IObjectInteractor objectInteractor
                                         , IMotionController playerMotion
                                         , ICharacterInfo playerInfo
                                         , Animator stateAnimator
                                         , PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(owningAvatar, stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);

        _aimingStateHash = AnimatorState.GetHash(owningAvatar, CurrentState);
        Assert.IsTrue(StateAnimator.HasState(0, _aimingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
    }

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector3.zero);
        StateAnimator.Play(_aimingStateHash);
    }

    public override void Aim(Vector3 aim) 
    { 
        if(Vector3.zero == aim) 
            StateController.ChangeState(LowerStateType.Idle);
    }

    public override void OnAir() 
    {
        StateController.ChangeState(LowerStateType.Jumping); 
    }

    public override void OnDamaged()
    {
        StateController.ChangeState(LowerStateType.Damaged); 
    }


    /****** Private Members ******/

    private int _aimingStateHash;
}
