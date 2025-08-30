using UnityEngine;

public interface IPlayerState<TState> where TState : StateType
{
    PlayerAvatarType OwningAvatar { get; }
    TState CurrentState { get; }
    void InitializeState(PlayerAvatarType owningAvatar
                        , IStateController<TState> stateController
                        , IObjectInteractor objectInteractor
                        , IMotionController playerMotion
                        , ICharacterInfo playerInfo     
                        , Animator stateAnimator
                        , PlayerWeaponBase playerWeapon
                        , ControlInputBuffer inputBuffer);
}