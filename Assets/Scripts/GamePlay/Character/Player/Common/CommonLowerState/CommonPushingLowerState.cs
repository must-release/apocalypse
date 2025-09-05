using UnityEngine;
using UnityEngine.Assertions;

namespace AD.GamePlay
{
    public class CommonPushingLowerState : PlayerLowerState
    {
        /****** Public Members ******/

        public override LowerStateType CurrentState => LowerStateType.Pushing;
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

            _pushingStateHash = AnimatorState.GetHash(OwningAvatar, CurrentState);
            Debug.Assert(StateAnimator.HasState(0, _pushingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
        }

        public override void OnEnter()
        {
            StateAnimator.Play(_pushingStateHash);
            StateAnimator.Update(0.0f);
        }

        public override void Move(HorizontalDirection horizontalInput)
        {
            if (HorizontalDirection.None == horizontalInput)
            {
                StateAnimator.speed = 0.0f;
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

        public override void StartJump()
        {
            PlayerMovement.SetVelocity(new Vector2(PlayerMovement.CurrentVelocity.x, PlayerStats.JumpingSpeed));
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

        private int _pushingStateHash;
    }
}
