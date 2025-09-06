using AD.GamePlay;
using UnityEngine;

public interface IPlayerState<TState> where TState : StateType
{
    PlayerAvatarType OwningAvatar { get; }
    TState CurrentState { get; }
    void InitializeState(PlayerAvatarType owningAvatar
                        , IStateController<TState> stateController
                        , IObjectInteractor objectInteractor
                        , CharacterMovement playerMovement
                        , PlayerCharacterStats playerStats     
                        , Animator stateAnimator
                        , PlayerWeaponBase playerWeapon
                        , ControlInputBuffer inputBuffer);
}