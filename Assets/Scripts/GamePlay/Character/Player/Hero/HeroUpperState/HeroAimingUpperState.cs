using UnityEngine;

namespace AD.GamePlay
{
    public class HeroAimingUpperState : PlayerUpperState
    {
        /****** Public Members ******/

        public override UpperStateType CurrentState => HeroUpperStateType.Aiming;

        public override void InitializeState(PlayerAvatarType owningAvatar
                                            , IStateController<UpperStateType> stateController
                                            , IObjectInteractor objectInteractor
                                            , CharacterMovement playerMovement
                                            , CharacterStats playerStats
                                            , Animator stateAnimator
                                            , PlayerWeaponBase playerWeapon
                                            , ControlInputBuffer inputBuffer)
        {
            base.InitializeState(owningAvatar, stateController, objectInteractor, playerMovement, playerStats, stateAnimator, playerWeapon, inputBuffer);

            Debug.Assert(PlayerAvatarType.Hero == owningAvatar, $"State {CurrentState} can only be used by Hero avatar.");
            Debug.Assert(StateAnimator.HasState(0, _AimingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
        }

        public override void OnEnter()
        {
            StateAnimator.Play(_AimingStateHash);
            StateAnimator.Update(0.0f);

            _aimingPosition = Vector3.zero;
        }

        public override void OnUpdate()
        {
            SetDirection();

            PlayerWeapon.RotateWeaponPivot(_aimingPosition);
        }

        public override void OnFixedUpdate()
        {
            PlayerWeapon.Aim(true);
        }

        public override void OnExit(UpperStateType nextState)
        {
            PlayerWeapon.Aim(false);

            if (HeroUpperStateType.AimAttacking != nextState)
                PlayerWeapon.SetWeaponPivotRotation(0);
        }

        public override void Aim(Vector3 aim)
        {
            if (Vector3.zero == aim)
                StateController.ChangeState(HeroUpperStateType.Idle);

            _aimingPosition = aim;
        }

        public override void Attack()
        {
            StateController.ChangeState(HeroUpperStateType.AimAttacking);
        }

        public override void OnAir()
        {
            StateController.ChangeState(HeroUpperStateType.Jumping);
        }

        public override void Disable()
        {
            StateController.ChangeState(UpperStateType.Disabled);
        }


        /****** Private Members ******/

        private readonly int _AimingStateHash = AnimatorState.GetHash(PlayerAvatarType.Hero, HeroUpperStateType.Aiming);

        private Vector3 _aimingPosition;

        private void SetDirection()
        {
            var direction = PlayerMovement.CurrentPosition.x < _aimingPosition.x ? FacingDirection.Right : FacingDirection.Left;
            if (direction != PlayerMovement.CurrentFacingDirection)
                PlayerMovement.SetFacingDirection(direction);
        }
    }
}
