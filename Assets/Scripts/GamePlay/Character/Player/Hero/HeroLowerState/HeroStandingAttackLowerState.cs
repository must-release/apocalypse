using UnityEngine;

namespace AD.GamePlay
{
    public class HeroStandingAttackLowerState : PlayerLowerState
    {
        public override LowerStateType CurrentState => HeroLowerStateType.StandingAttack;
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

            Debug.Assert(PlayerAvatarType.Hero == owningAvatar, "HeroStandingAttackLowerState can only be used by Hero avatar.");
            _standingAttackStateHash = AnimatorState.GetHash(owningAvatar, CurrentState);
            Debug.Assert(StateAnimator.HasState(0, _standingAttackStateHash), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
        }

        public override void OnEnter()
        {
            StateAnimator.Play(_standingAttackStateHash, 0, 0);
            StateAnimator.Update(0f);

            _attackCoolTime         = PlayerWeapon.Attack();
            _shouldContinueAttack   = false;
        }

        public override void OnUpdate()
        {
            _attackCoolTime -= Time.deltaTime;

            if (0 < _attackCoolTime) return;

            var nextState = _shouldContinueAttack ? HeroLowerStateType.StandingAttack : HeroLowerStateType.Idle;
            StateController.ChangeState(nextState);
        }

        public override void Attack()
        {
            _shouldContinueAttack = true;
        }

        public override void Move(HorizontalDirection horizontalInput)
        {
            if (HorizontalDirection.None != horizontalInput)
            {
                StateController.ChangeState(HeroLowerStateType.Running);
            }
        }

        public override void UpDown(VerticalDirection verticalInput)
        {

        }

        public override void OnDamaged()
        {
            StateController.ChangeState(HeroLowerStateType.Damaged);
        }

        public override void OnAir()
        {
            StateController.ChangeState(HeroLowerStateType.Jumping);
        }

        public override void StartJump()
        {
            PlayerMovement.SetVelocity(new Vector2(PlayerMovement.CurrentVelocity.x, PlayerStats.JumpingSpeed));
            StateController.ChangeState(HeroLowerStateType.Jumping);
        }


        /****** Private Members ******/

        private int _standingAttackStateHash;
        
        private float   _attackCoolTime;
        private bool    _shouldContinueAttack;
    }
}