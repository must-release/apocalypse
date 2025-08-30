using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

namespace AD.GamePlay
{
    public class HeroIdleLookingUpLowerState : PlayerLowerState
    {
        /****** Public Members ******/

        public override LowerStateType CurrentState => HeroLowerStateType.IdleLookingUp;
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

            Debug.Assert(PlayerAvatarType.Hero == owningAvatar, $"State {CurrentState} can only be used by Hero avatar.");
            Debug.Assert(StateAnimator.HasState(0, _IdleLookingUpStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
        }

        public override void OnEnter()
        {
            StateAnimator.Play(_IdleLookingUpStateHash);
            StateAnimator.Update(0.0f);
        }

        public override void Attack()
        {
            StateController.ChangeState(HeroLowerStateType.IdleTopAttacking);
        }

        public override void UpDown(VerticalDirection verticalDirection)
        {
            if (verticalDirection != VerticalDirection.Up)
            {
                StateController.ChangeState(HeroLowerStateType.Idle);
            }
        }

        public override void Move(HorizontalDirection horizontalInput)
        {
            if (HorizontalDirection.None != horizontalInput)
            {
                StateController.ChangeState(HeroLowerStateType.Running);
            }
        }

        public override void OnAir()
        {
            StateController.ChangeState(HeroLowerStateType.Jumping);
        }


        /****** Private Members ******/

        private readonly int _IdleLookingUpStateHash = AnimatorState.GetHash(PlayerAvatarType.Hero, HeroLowerStateType.IdleLookingUp);
    }
}
