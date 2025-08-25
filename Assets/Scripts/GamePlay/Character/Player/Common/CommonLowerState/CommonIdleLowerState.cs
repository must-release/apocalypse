using UnityEngine;
using UnityEngine.Assertions;

public class CommonIdleLowerState : PlayerLowerState
{

    public override LowerStateType CurrentState => LowerStateType.Idle;
    public override bool ShouldDisableUpperBody => false;

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

        _idleStateHash = AnimatorState.GetHash(OwningAvatar, CurrentState);
        Debug.Assert(StateAnimator.HasState(0, _idleStateHash), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_idleStateHash);
        StateAnimator.Update(0.0f);
        PlayerMotion.SetVelocity(Vector2.zero);
    }

    public override void StartJump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed));
        StateController.ChangeState(LowerStateType.Jumping);
    }

    public override void OnAir()
    {
        StateController.ChangeState(LowerStateType.Jumping);
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim) return;

        StateController.ChangeState(LowerStateType.Aiming);
    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        if (HorizontalDirection.None == horizontalInput)
            return;

        StateController.ChangeState(LowerStateType.Running);
    }

    public override void Tag()
    {
        StateController.ChangeState(LowerStateType.Tagging);
    }

    public override void UpDown(VerticalDirection verticalInput)
    {
        if (null == ObjectInteractor.CurrentClimbableObject || VerticalDirection.None == verticalInput)
            return;

        var refPos = ObjectInteractor.CurrentClimbableObject.GetClimbReferencePoint();
        var curPos = PlayerInfo.CurrentPosition;

        if ((curPos.y < refPos.y && VerticalDirection.Up == verticalInput) || (refPos.y < curPos.y && VerticalDirection.Down == verticalInput))
        {
            StateController.ChangeState(LowerStateType.Climbing);
        }
    }

    public override void OnDamaged()
    {
        StateController.ChangeState(LowerStateType.Damaged);
    }


    /****** Private Members ******/

    private int _idleStateHash;
}