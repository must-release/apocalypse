using UnityEngine;
using UnityEngine.Assertions;

namespace AD.GamePlay
{
    public class CommonClimbingLowerState : PlayerLowerState
    {
        /****** Public Members ******/

        public override LowerStateType CurrentState => LowerStateType.Climbing;
        public override bool ShouldDisableUpperBody => true;

        public override void InitializeState(PlayerAvatarType owningAvatar
                                            , IStateController<LowerStateType> stateController
                                            , IObjectInteractor objectInteractor
                                            , CharacterMovement playerMovement
                                            , CharacterStats playerStats
                                            , Animator stateAnimator
                                            , PlayerWeaponBase playerWeapon)
        {
            base.InitializeState(owningAvatar, stateController, objectInteractor, playerMovement, playerStats, stateAnimator, playerWeapon);

            _climbingSpeed      = playerStats.MovingSpeed * 2.0f / 3.0f;
            _climbUpHeight      = 0.1f;
            _climbDownHeight    = playerStats.CharacterHeight / 2 + 0.2f;

            _climbingDownStateHash = AnimatorState.GetHash(owningAvatar, LowerStateType.Climbing, "Down");
            _climbingUpStateHash   = AnimatorState.GetHash(owningAvatar, LowerStateType.Climbing, "Up");

            Debug.Assert(StateAnimator.HasState(0, _climbingDownStateHash), $"Animator of {owningAvatar} does not have {CurrentState} down lower state.");
            Debug.Assert(StateAnimator.HasState(0, _climbingUpStateHash), $"Animator of {owningAvatar} does not have {CurrentState} up lower state.");
        }

        public override void OnEnter()
        {
            _recentMoveDirection = VerticalDirection.None;

            PlayerMovement.SetGravityScale(0);
            StateAnimator.Play(_climbingUpStateHash);

            _climbingObject = ObjectInteractor.CurrentClimbableObject;
            _climbingObject.OnClimbStart(ObjectInteractor);

            MoveNearToClimbingObject();
        }

        public override void OnUpdate()
        {
            ControlClimbingAnimation();
            CheckIfCanClimbFurther();
        }

        public override void OnExit(LowerStateType _)
        {
            PlayerMovement.SetGravityScale(PlayerStats.Gravity);
            _climbingObject.OnClimbEnd(ObjectInteractor); 
            StateAnimator.speed = 1.0f;
        }

        public override void UpDown(VerticalDirection verticalInput)
        {
            PlayerMovement.SetVelocity(Vector2.up * (int)verticalInput * _climbingSpeed);
            _recentMoveDirection = verticalInput;
        }

        public override void StartJump()
        {
            PlayerMovement.SetVelocity(new Vector2(PlayerMovement.CurrentVelocity.x, PlayerStats.JumpingSpeed / 3));

            StateController.ChangeState(LowerStateType.Jumping);
        }

        public override void OnDamaged()
        {
            StateController.ChangeState(LowerStateType.Damaged);
        }

        public override void OnGround()
        {
            StateController.ChangeState(LowerStateType.Idle);
        }


        /****** Private Members ******/

        private VerticalDirection   _recentMoveDirection;
        private IClimbable          _climbingObject;

        private int     _climbingUpStateHash;
        private int     _climbingDownStateHash;
        private float   _climbingSpeed;
        private float   _climbUpHeight;
        private float   _climbDownHeight;

        private void MoveNearToClimbingObject()
        {
            float offset = (PlayerMovement.CurrentPosition.y < _climbingObject.GetClimbReferencePoint().y) ? _climbUpHeight :  -_climbDownHeight;

            PlayerMovement.TeleportTo(new Vector2(_climbingObject.GetClimbReferencePoint().x, PlayerMovement.CurrentPosition.y + offset));
        }

        private void ControlClimbingAnimation()
        {
            if (VerticalDirection.None == _recentMoveDirection)
            {
                StateAnimator.speed = 0.0f;
            }
            else
            {
                var nextClipHash = (VerticalDirection.Up == _recentMoveDirection) ? _climbingUpStateHash : _climbingDownStateHash;

                if (StateAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != nextClipHash)
                {
                    StateAnimator.Play(nextClipHash);
                }

                StateAnimator.speed = 1.0f;
            }
        }

        private void CheckIfCanClimbFurther()
        {
            if (VerticalDirection.None == _recentMoveDirection || _climbingObject.CanClimbFurther(PlayerMovement.CurrentPosition, _recentMoveDirection))
                return;

            if (VerticalDirection.Up == _recentMoveDirection)
            {
                // Movement player on the upside of the ladder
                PlayerMovement.TeleportTo(PlayerMovement.CurrentPosition + Vector3.up * (PlayerStats.CharacterHeight / 2 + 0.8f));
                StateController.ChangeState(LowerStateType.Idle);
            }
            else
            {
                // Climbed down the climbing object
                var nextState = PlayerMovement.StandingGround == null ? LowerStateType.Jumping : LowerStateType.Idle;
                StateController.ChangeState(nextState);
            }
        }
    }
}
