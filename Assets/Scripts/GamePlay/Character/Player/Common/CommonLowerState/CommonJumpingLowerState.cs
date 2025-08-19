using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class CommonJumpingLowerState : PlayerLowerState
{
    /****** Public Members ******/

    public override LowerStateType CurrentState => LowerStateType.Jumping;
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

        _jumpingStartStateHash  = AnimatorState.GetHash(owningAvatar, LowerStateType.Jumping, "Start");
        _jumpingLoopStateHash   = AnimatorState.GetHash(owningAvatar, LowerStateType.Jumping, "Loop");
        _jumpingEndStateHash    = AnimatorState.GetHash(owningAvatar, LowerStateType.Jumping, "End");

        Debug.Assert(StateAnimator.HasState(0, _jumpingStartStateHash), $"Animator of {owningAvatar} does not have jumping start lower state.");
        Debug.Assert(StateAnimator.HasState(0, _jumpingLoopStateHash), $"Animator of {owningAvatar} does not have jumping loop lower state.");
        Debug.Assert(StateAnimator.HasState(0, _jumpingEndStateHash), $"Animator of {owningAvatar} does not have jumping end lower state.");
    }

    public override void OnEnter()
    {
        if (0 < PlayerInfo.CurrentVelocity.y)
        {
            StateAnimator.Play(_jumpingStartStateHash);
            _isStartingJump = true;
            _isJumping = true;
        }
        else
        {
            StateAnimator.Play(_jumpingLoopStateHash);
            _isStartingJump = false;
            _isJumping = false;
        }

        StateAnimator.Update(0);
    }

    public override void OnUpdate()
    {
        if (0 < PlayerInfo.CurrentVelocity.y && false == _isJumping)
            ApplyJumpCut();

        if (_isStartingJump)
            ChangeToJumpingLoopAnimation();

    }

    public override void OnExit(LowerStateType _)
    {
        if (null != _landCoroutine)
        {
            StopCoroutine(_landCoroutine);
            _landCoroutine = null;
        }
    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        if (HorizontalDirection.None == horizontalInput)
        {
            PlayerMotion.SetVelocity(new Vector2(0, PlayerInfo.CurrentVelocity.y));
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

    public override void OnGround()
    {
        _landCoroutine = StartCoroutine(LandOnGround());
    }

    public override void UpDown(VerticalDirection verticalInput)
    {
        if (null == ObjectInteractor.CurrentClimbableObject || VerticalDirection.Up != verticalInput)
            return;

        StateController.ChangeState(LowerStateType.Climbing);
    }

    public override void CheckJumping(bool isJumping)
    {
        if (false == _isJumping)
            return;


        if (false == isJumping)
            _isJumping = false;
    }

    public override void OnDamaged()
    {
        StateController.ChangeState(LowerStateType.Damaged);
    }


    /****** Private Members ******/

    private const float _JumpFallSpeed = 10f;

    private Coroutine _landCoroutine;

    private int     _jumpingStartStateHash;
    private int     _jumpingLoopStateHash;
    private int     _jumpingEndStateHash;
    private bool    _isStartingJump;
    private bool    _isJumping;

    private void ApplyJumpCut()
    {
        PlayerMotion.SetVelocity(new Vector2(
            PlayerInfo.CurrentVelocity.x,
            PlayerInfo.CurrentVelocity.y - _JumpFallSpeed * PlayerInfo.JumpingSpeed * Time.deltaTime
        ));
    }

    private void ChangeToJumpingLoopAnimation()
    {
        if (StateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return;

        StateAnimator.Play(_jumpingLoopStateHash);
        StateAnimator.Update(0.0f);

        _isStartingJump = false;
    }

    private IEnumerator LandOnGround()
    {
        StateAnimator.Play(_jumpingEndStateHash);
        StateAnimator.Update(0.0f);

        yield return new WaitWhile(() => StateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);

        StateController.ChangeState(LowerStateType.Idle);
    }
}
