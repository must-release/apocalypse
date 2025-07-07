using UnityEngine;
using UnityEngine.Assertions;

public class CommonRunningLowerState : PlayerLowerState
{
    /****** Public Members ******/

    public override LowerStateType CurrentState => LowerStateType.Running;
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

        _runningStateHash = AnimatorState.GetHash(OwningAvatar, CurrentState);
        Assert.IsTrue(StateAnimator.HasState(0, _runningStateHash), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_runningStateHash);
        StateAnimator.Update(0.0f);
    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        if (HorizontalDirection.None == horizontalInput)
        {
            StateController.ChangeState(LowerStateType.Idle);
            return;
        }

        // Movement player
        PlayerMotion.SetVelocity(new Vector2((int)horizontalInput * PlayerInfo.MovingSpeed, PlayerInfo.CurrentVelocity.y));

        // set direction
        FacingDirection direction = (HorizontalDirection.Left == horizontalInput) ? FacingDirection.Left : FacingDirection.Right;
        if (direction != PlayerInfo.CurrentFacingDirection)
        {
            PlayerMotion.SetFacingDirection(direction);
        }
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


    /******* Private Members ******/

    private int _runningStateHash;
}
