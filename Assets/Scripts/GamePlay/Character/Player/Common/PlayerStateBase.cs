using UnityEngine;

namespace AD.GamePlay
{
    public abstract class PlayerStateBase<TState> : MonoBehaviour, IPlayerState<TState> where TState : StateType
    {
        /****** Public Members ******/
        public PlayerAvatarType OwningAvatar { get; private set; }
        public abstract TState CurrentState { get; }

        public virtual void InitializeState(PlayerAvatarType owningAvatar
                                            , IStateController<TState> stateController
                                            , IObjectInteractor objectInteractor
                                            , CharacterMovement playerMovement
                                            , CharacterStats playerStats
                                            , Animator stateAnimator
                                            , PlayerWeaponBase playerWeapon
                                            , ControlInputBuffer inputBuffer
        )
        {
            Debug.Assert(null != stateController, $"StateController in {CurrentState} is not assigned.");
            Debug.Assert(null != objectInteractor, $"ObjectInteractor in {CurrentState} is not assigned.");
            Debug.Assert(null != playerMovement, $"PlayerMovement in {CurrentState} is not assigned.");
            Debug.Assert(null != playerStats, $"PlayerStats in {CurrentState} is not assigned.");
            Debug.Assert(null != stateAnimator, $"StateAnimator in {CurrentState} is not assigned.");
            Debug.Assert(null != playerWeapon, $"PlayerWeapon in {CurrentState} is not assigned.");
            Debug.Assert(null != inputBuffer, $"InputBuffer in {CurrentState} is not assigned");

            OwningAvatar        = owningAvatar;
            StateController     = stateController;
            ObjectInteractor    = objectInteractor;
            PlayerMovement      = playerMovement;
            PlayerStats         = playerStats;
            PlayerWeapon        = playerWeapon;
            StateAnimator       = stateAnimator;
            InputBuffer         = inputBuffer;
        }

        /****** Protected Members ******/

        protected IStateController<TState>  StateController     { get; private set; }
        protected IObjectInteractor         ObjectInteractor    { get; private set; }
        protected CharacterMovement         PlayerMovement      { get; private set; }
        protected CharacterStats            PlayerStats         { get; private set; }
        protected PlayerWeaponBase          PlayerWeapon        { get; private set; }
        protected Animator                  StateAnimator       { get; private set; }
        protected ControlInputBuffer        InputBuffer         { get; private set; }
    }
}