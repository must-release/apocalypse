using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AD.GamePlay
{
    public class CommonJumpingLowerState : PlayerLowerState
    {
        /****** Public Members ******/

        public override LowerStateType CurrentState => LowerStateType.Jumping;
        public override bool ShouldDisableUpperBody => true;

        public override void InitializeState(PlayerAvatarType owningAvatar
                                            , IStateController<LowerStateType> stateController
                                            , IObjectInteractor objectInteractor
                                            , CharacterMovement playerMovement
                                            , CharacterStats playerStats
                                            , Animator stateAnimator
                                            , PlayerWeaponBase playerWeapon
                                            , ControlInputBuffer inputBuffer)
        {
            base.InitializeState(owningAvatar, stateController, objectInteractor, playerMovement, playerStats, stateAnimator, playerWeapon, inputBuffer);

            _jumpingStartStateHash  = AnimatorState.GetHash(owningAvatar, LowerStateType.Jumping, "Start");
            _jumpingLoopStateHash   = AnimatorState.GetHash(owningAvatar, LowerStateType.Jumping, "Loop");
            _jumpingEndStateHash    = AnimatorState.GetHash(owningAvatar, LowerStateType.Jumping, "End");

            Debug.Assert(StateAnimator.HasState(0, _jumpingStartStateHash), $"Animator of {owningAvatar} does not have jumping start lower state.");
            Debug.Assert(StateAnimator.HasState(0, _jumpingLoopStateHash), $"Animator of {owningAvatar} does not have jumping loop lower state.");
            Debug.Assert(StateAnimator.HasState(0, _jumpingEndStateHash), $"Animator of {owningAvatar} does not have jumping end lower state.");
        }

        public override void OnEnter()
        {
            if (0 < PlayerMovement.CurrentVelocity.y)
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
            if (0 < PlayerMovement.CurrentVelocity.y && false == _isJumping)
                ApplyJumpCut();

            if (_isStartingJump)
                ChangeToJumpingLoopAnimation();

        }

        public override void OnExit(LowerStateType _)
        {
            if (null != _landOnGroundCTS)
            {
                _landOnGroundCTS.Cancel();
                _landOnGroundCTS.Dispose();
                _landOnGroundCTS = null;
            }
        }

        public override void Move(HorizontalDirection horizontalInput)
        {
            if (HorizontalDirection.None == horizontalInput)
            {
                PlayerMovement.SetVelocity(new Vector2(0, PlayerMovement.CurrentVelocity.y));
                return;
            }

            // Movement player
            PlayerMovement.SetVelocity(new Vector2((int)horizontalInput * PlayerStats.MovingSpeed, PlayerMovement.CurrentVelocity.y));

            // set direction
            FacingDirection direction = (HorizontalDirection.Left == horizontalInput) ? FacingDirection.Left : FacingDirection.Right;
            if (direction != PlayerMovement.CurrentFacingDirection)
            {
                PlayerMovement.SetFacingDirection(direction);
            }
        }

        public override void OnGround()
        {
            if (InputBuffer.HasBufferedJump)
            {
                InputBuffer.ConsumeJumpBuffer();
                PlayerMovement.SetVelocity(new Vector2(PlayerMovement.CurrentVelocity.x, PlayerStats.JumpingSpeed));
                StateController.ChangeState(LowerStateType.Jumping);
            }
            else
            {
                _landOnGroundCTS = new CancellationTokenSource();
                LandOnGroundAsync(_landOnGroundCTS.Token).Forget();
            }
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

        public override void StartJump()
        {
            if (null != _landOnGroundCTS)
            {
                _landOnGroundCTS.Cancel();
                
                InputBuffer.BufferJump();
                InputBuffer.ConsumeJumpBuffer();
                PlayerMovement.SetVelocity(new Vector2(PlayerMovement.CurrentVelocity.x, PlayerStats.JumpingSpeed));
                StateController.ChangeState(LowerStateType.Jumping);
            }
            else
            {
                InputBuffer.BufferJump();
            }
        }


        /****** Private Members ******/

        private const float _JumpFallSpeed = 10f;

        private CancellationTokenSource _landOnGroundCTS;

        private int     _jumpingStartStateHash;
        private int     _jumpingLoopStateHash;
        private int     _jumpingEndStateHash;
        private bool    _isStartingJump;
        private bool    _isJumping;

        private void ApplyJumpCut()
        {
            PlayerMovement.SetVelocity(new Vector2(
                PlayerMovement.CurrentVelocity.x,
                PlayerMovement.CurrentVelocity.y - _JumpFallSpeed * PlayerStats.JumpingSpeed * Time.deltaTime
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

        private async UniTaskVoid LandOnGroundAsync(CancellationToken cancellationToken)
        {
            StateAnimator.Play(_jumpingEndStateHash);
            StateAnimator.Update(0.0f);

            await UniTask.WaitWhile(() => StateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f, cancellationToken: cancellationToken);

            StateController.ChangeState(LowerStateType.Idle);
        }
    }
}