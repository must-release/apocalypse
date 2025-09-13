using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.GamePlay
{
    public class CommonRunningLowerState : PlayerLowerState
    {
        /****** Public Members ******/

        public override LowerStateType CurrentState => LowerStateType.Running;
        public override bool ShouldDisableUpperBody => false;

        public override void InitializeState(PlayerAvatarType owningAvatar
                            , IStateController<LowerStateType> stateController
                            , IObjectInteractor objectInteractor
                            , CharacterMovement playerMovement
                            , PlayerCharacterStats playerStats
                            , Animator stateAnimator
                            , PlayerWeaponBase playerWeapon
                            , ControlInputBuffer inputBuffer)
        {
            base.InitializeState(owningAvatar, stateController, objectInteractor, playerMovement, playerStats, stateAnimator, playerWeapon, inputBuffer);

            _runningStateHash = AnimatorState.GetHash(OwningAvatar, CurrentState);
            Debug.Assert(StateAnimator.HasState(0, _runningStateHash), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
        }

        public override void OnEnter()
        {
            StateAnimator.Play(_runningStateHash);
            StateAnimator.Update(0.0f);
            
            _stateExitCTS = new CancellationTokenSource();
        }

        public override void OnExit(LowerStateType nextState)
        {
            _stateExitCTS?.Cancel();
            _stateExitCTS?.Dispose();
            _stateExitCTS = null;
        }

        public override void Move(HorizontalDirection horizontalInput)
        {
            if (HorizontalDirection.None == horizontalInput)
            {
                StateController.ChangeState(LowerStateType.Idle);
                return;
            }

            FacingDirection direction = (HorizontalDirection.Left == horizontalInput) ? FacingDirection.Left : FacingDirection.Right;
            if (direction != PlayerMovement.CurrentFacingDirection)
            {
                PlayerMovement.SetFacingDirection(direction);
            }

            if (CanPushObject(horizontalInput))
            {
                StateController.ChangeState(LowerStateType.Pushing);
                return;
            }

            PlayerMovement.SetVelocity(new Vector2((int)horizontalInput * PlayerStats.MovingSpeed, PlayerMovement.CurrentVelocity.y));
        }

        public override void StartJump()
        {
            PlayerMovement.SetVelocity(new Vector2(PlayerMovement.CurrentVelocity.x, PlayerStats.JumpingSpeed));
            StateController.ChangeState(LowerStateType.Jumping);
        }

        public override void OnAir()
        {
            Debug.Assert(null != _stateExitCTS, $"State exit cancellation token source is null in {CurrentState}");

            WaitForCoyoteJump(_stateExitCTS.Token).Forget();
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
            var curPos = PlayerMovement.CurrentPosition;

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

        private CancellationTokenSource _stateExitCTS;
        private int _runningStateHash;

        private void OnDestroy()
        {
            _stateExitCTS?.Cancel();
            _stateExitCTS?.Dispose();
        }

        private bool CanPushObject(HorizontalDirection horizontalInput)
        {
            if (null == ObjectInteractor.CurrentPushableObject)
                return false;

            var playerPos = PlayerMovement.CurrentPosition;
            var objectPos = ObjectInteractor.CurrentPushableObject.CurrentPosition;

            // Check horizontal direction alignment
            var objectLocatedDirection = (playerPos.x < objectPos.x) ? HorizontalDirection.Right : HorizontalDirection.Left;
            if (horizontalInput != objectLocatedDirection)
                return false;

            // Check vertical position within character height range
            var halfCharacterHeight = PlayerStats.CharacterHeight * 0.5f;
            var verticalDistance = Mathf.Abs(playerPos.y - objectPos.y);

            return verticalDistance <= halfCharacterHeight;
        }

        private async UniTask WaitForCoyoteJump(CancellationToken cancellationToken)
        {
            InputBuffer.StartCoyoteJump();

            bool isCanceled = await UniTask.WaitWhile(() => InputBuffer.CanCoyoteJump, cancellationToken: cancellationToken).SuppressCancellationThrow();
            if (false == isCanceled)
            {
                StateController.ChangeState(LowerStateType.Jumping);
            }
        }
    }
}
