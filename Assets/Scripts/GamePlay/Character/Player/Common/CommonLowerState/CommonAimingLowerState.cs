using UnityEngine;
using UnityEngine.Assertions;

namespace AD.GamePlay
{
    public class CommonAimingLowerState : PlayerLowerState
    {
        public override LowerStateType CurrentState => LowerStateType.Aiming;
        public override bool ShouldDisableUpperBody => false;

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

            _aimingStateHash = AnimatorState.GetHash(owningAvatar, CurrentState);
            Debug.Assert(StateAnimator.HasState(0, _aimingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
        }

        public override void OnEnter()
        {
            PlayerMovement.SetVelocity(Vector3.zero);
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
}
