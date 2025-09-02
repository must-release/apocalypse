using UnityEngine;
using UnityEngine.Assertions;

namespace AD.GamePlay
{
    public class CommonRunningUpperState : PlayerUpperState
    {
        /****** Public Members ******/

        public override UpperStateType CurrentState => UpperStateType.Running;

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

            _RunningStateHash = AnimatorState.GetHash(OwningAvatar, CurrentState);
            Debug.Assert(StateAnimator.HasState(0, _RunningStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
        }

        public override void OnEnter()
        {
            StateAnimator.Play(_RunningStateHash, 0, LowerStateInfo.AnimationNormalizedTime);
        }

        public override void LookUp(bool lookUp)
        {
            if (false == lookUp) return;

            StateController.ChangeState(UpperStateType.LookingUp);
        }

        public override void Move(HorizontalDirection horizontalInput)
        {
            if (HorizontalDirection.None != horizontalInput)
                return;

            StateController.ChangeState(UpperStateType.Idle);
        }

        public override void Disable()
        {
            StateController.ChangeState(UpperStateType.Disabled);
        }


        /****** Private Members ******/

        private int _RunningStateHash;
    }
}
