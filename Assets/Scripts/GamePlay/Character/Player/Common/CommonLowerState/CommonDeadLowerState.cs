using UnityEngine;
using UnityEngine.Assertions;

namespace AD.GamePlay
{
    public class CommonDeadLowerState : PlayerLowerState
    {
        /****** Public Members ******/

        public override LowerStateType CurrentState => LowerStateType.Dead;
        public override bool ShouldDisableUpperBody => true; 

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

            _deadStateHash = AnimatorState.GetHash(owningAvatar, CurrentState);
            Debug.Assert(StateAnimator.HasState(0, AnimatorState.GetHash(owningAvatar, CurrentState)), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
        }

        public override void OnEnter()
        {
            PlayerMovement.SetVelocity(Vector2.zero);
            StateAnimator.Play(_deadStateHash);
            StateAnimator.Update(0.0f);

            _isAnimationPlaying = true;
        }

        public override void OnUpdate()
        {
            if (false == _isAnimationPlaying) return;

            var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

            if (1.0f <= stateInfo.normalizedTime)
            {
                GameEventManager.Instance.Submit(GameEventFactory.CreateCommonEvent(CommonEventType.GameOver));
                _isAnimationPlaying = false;
            }
        }


        /****** Private Members ******/

        private int _deadStateHash;
        private bool _isAnimationPlaying;
    }
}
