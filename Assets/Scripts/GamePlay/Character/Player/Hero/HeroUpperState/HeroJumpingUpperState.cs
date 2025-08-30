using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.GamePlay
{
    public class HeroJumpingUpperState : PlayerUpperState
    {
        /****** Public Members ******/

        public override UpperStateType CurrentState => HeroUpperStateType.Jumping;

        public override void InitializeState(PlayerAvatarType owningAvatar
                                            , IStateController<UpperStateType> stateController
                                            , IObjectInteractor objectInteractor
                                            , CharacterMovement playerMovement
                                            , CharacterStats playerStats
                                            , Animator stateAnimator
                                            , PlayerWeaponBase playerWeapon)
        {
            base.InitializeState(owningAvatar, stateController, objectInteractor, playerMovement, playerStats, stateAnimator, playerWeapon);

            Debug.Assert(PlayerAvatarType.Hero == owningAvatar, $"State {CurrentState} can only be used by Hero avatar.");
            Debug.Assert(StateAnimator.HasState(0, _JumpingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
        }

        public override void OnEnter()
        {
            StateAnimator.Play(_JumpingStateHash);
        }

        public override void Disable()
        {
            StateController.ChangeState(UpperStateType.Disabled);
        }

        public override void OnGround()
        {
            ChangeToIdleAsync().Forget();
        }

        public override void Attack()
        {
            StateController.ChangeState(HeroUpperStateType.Attacking);
        }



        /****** Private Members ******/

        private readonly int _JumpingStateHash = AnimatorState.GetHash(PlayerAvatarType.Hero, HeroUpperStateType.Jumping);

        private async UniTask ChangeToIdleAsync()
        {
            await UniTask.WaitWhile(() => LowerStateInfo.AnimationNormalizedTime < 1);

            StateController.ChangeState(HeroUpperStateType.Idle);
        }
    }
}
