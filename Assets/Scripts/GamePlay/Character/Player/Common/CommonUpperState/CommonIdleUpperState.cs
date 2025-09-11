using UnityEngine;
using UnityEngine.Assertions;

namespace AD.GamePlay
{
    public class CommonIdleUpperState : PlayerUpperState
    {
        /****** Public Members ******/

        public override UpperStateType CurrentState => UpperStateType.Idle;

        public override void InitializeState(PlayerAvatarType owningAvatar
                                            , IStateController<UpperStateType> stateController
                                            , IObjectInteractor objectInteractor
                                            , CharacterMovement playerMovement
                                            , PlayerCharacterStats playerStats
                                            , Animator stateAnimator
                                            , PlayerWeaponBase playerWeapon
                                            , ControlInputBuffer inputBuffer)
        {
            base.InitializeState(owningAvatar, stateController, objectInteractor, playerMovement, playerStats, stateAnimator, playerWeapon, inputBuffer);

            _IdleStateHash = AnimatorState.GetHash(owningAvatar, CurrentState);
            Debug.Assert(StateAnimator.HasState(0, _IdleStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
        }

        public override void OnEnter()
        {
            StateAnimator.Play(_IdleStateHash, 0, LowerStateInfo.AnimationNormalizedTime);
            StateAnimator.Update(0.0f);
        }

        public override void Move(HorizontalDirection horizontalInput)
        {
            if (HorizontalDirection.None == horizontalInput)
                return;

            StateController.ChangeState(UpperStateType.Running); 
        }

        public override void LookUp(bool lookUp) 
        { 
            if (false == lookUp) return;

            StateController.ChangeState(UpperStateType.LookingUp);
        }

        public override void Disable()
        {
            StateController.ChangeState(UpperStateType.Disabled);
        }


        /****** Private Members ******/

        private int _IdleStateHash;
    }
}
