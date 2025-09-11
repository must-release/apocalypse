using NUnit.Framework;
using UnityEngine;

namespace AD.GamePlay
{
    public class HeroAimAttackingUpperState : PlayerUpperState
    {
        /****** Public Members ******/

        public override UpperStateType CurrentState => HeroUpperStateType.AimAttacking;

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
            Debug.Assert(StateAnimator.HasState(0, _AimAttackingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
        }

        public override void OnEnter()
        {
            StateAnimator.Play(_AimAttackingStateHash);

            _postDelay = PlayerWeapon.Attack();
        }

        public override void OnUpdate()
        {
            var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

            _postDelay -= Time.deltaTime;

            if (1.0f <= stateInfo.normalizedTime && _postDelay < 0)
            {
                StateController.ChangeState(HeroUpperStateType.Aiming);
            }
        }

        public override void OnExit(UpperStateType nextState)
        {
            if (HeroUpperStateType.Aiming != nextState)
            {
                PlayerWeapon.SetWeaponPivotRotation(0);
                PlayerWeapon.Aim(false);
            }
        }

        public override void Aim(Vector3 aim)
        {
            if (Vector3.zero == aim)
                StateController.ChangeState(UpperStateType.Idle);
        }

        public override void Disable()
        {
            StateController.ChangeState(UpperStateType.Disabled);
        }


        /****** Private Members ******/

        private readonly int _AimAttackingStateHash = AnimatorState.GetHash(PlayerAvatarType.Hero, HeroUpperStateType.AimAttacking);

        private float _postDelay;
    }
}
