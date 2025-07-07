using UnityEngine;
using UnityEngine.Assertions;

public abstract class PlayerStateBase<TState> : MonoBehaviour, IPlayerState<TState> where TState : StateType
{
    /****** Public Members ******/
    public PlayerAvatarType OwningAvatar { get; private set; }
    public abstract TState CurrentState { get; }

    public virtual void InitializeState(PlayerAvatarType owningAvatar
                                        , IStateController<TState> stateController
                                        , IObjectInteractor objectInteractor
                                        , IMotionController playerMotion
                                        , ICharacterInfo playerInfo
                                        , Animator stateAnimator
                                        , PlayerWeaponBase playerWeapon
    )
    {
        Assert.IsTrue(null != stateController, $"StateController in {CurrentState} is not assigned.");
        Assert.IsTrue(null != objectInteractor, $"ObjectInteractor in {CurrentState} is not assigned.");
        Assert.IsTrue(null != playerMotion, $"PlayerMotion in {CurrentState} is not assigned.");
        Assert.IsTrue(null != playerInfo, $"PlayerInfo in {CurrentState} is not assigned.");
        Assert.IsTrue(null != stateAnimator, $"StateAnimator in {CurrentState} is not assigned.");
        Assert.IsTrue(null != playerWeapon, $"PlayerWeapon in {CurrentState} is not assigned.");

        OwningAvatar        = owningAvatar;
        StateController     = stateController;
        ObjectInteractor    = objectInteractor;
        PlayerMotion        = playerMotion;
        PlayerInfo          = playerInfo;
        PlayerWeapon        = playerWeapon;
        StateAnimator       = stateAnimator;
    }

    /****** Protected Members ******/

    protected IStateController<TState>      StateController     { get; private set; }
    protected IObjectInteractor             ObjectInteractor    { get; private set; }
    protected IMotionController             PlayerMotion        { get; private set; }
    protected ICharacterInfo                PlayerInfo          { get; private set; }
    protected PlayerWeaponBase              PlayerWeapon        { get; private set; }
    protected Animator                      StateAnimator       { get; private set; }
}