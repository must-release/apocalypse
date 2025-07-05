using UnityEngine;
using UnityEngine.Assertions;

public abstract class PlayerStateBase<TState> : MonoBehaviour where TState : StateType
{
    /****** Public Members ******/

    public abstract TState StateType { get; }

    public virtual void InitializeState(IStateController<TState> stateController,
                                        IObjectInteractor objectInteractor,
                                        IMotionController playerMotion,
                                        ICharacterInfo playerInfo,
                                        Animator stateAnimator,
                                        PlayerWeaponBase playerWeapon
    )
    {
        Assert.IsTrue(null != stateController, $"StateController in {StateType} is not assigned.");
        Assert.IsTrue(null != objectInteractor, $"ObjectInteractor in {StateType} is not assigned.");
        Assert.IsTrue(null != playerMotion, $"PlayerMotion in {StateType} is not assigned.");
        Assert.IsTrue(null != playerInfo, $"PlayerInfo in {StateType} is not assigned.");
        Assert.IsTrue(null != stateAnimator, $"StateAnimator in {StateType} is not assigned.");
        Assert.IsTrue(null != playerWeapon, $"PlayerWeapon in {StateType} is not assigned.");

        StateController     = stateController;
        ObjectInteractor    = objectInteractor;
        PlayerMotion        = playerMotion;
        PlayerInfo          = playerInfo;
        PlayerWeapon        = playerWeapon;
        StateAnimator       = stateAnimator;
    }

    /****** Protected Members ******/

    protected IStateController<TState>      StateController { get; private set; }
    protected IObjectInteractor             ObjectInteractor { get; private set; }
    protected IMotionController             PlayerMotion        { get; private set; }
    protected ICharacterInfo                PlayerInfo          { get; private set; }
    protected PlayerWeaponBase              PlayerWeapon        { get; private set; }
    protected Animator                      StateAnimator       { get; private set; }
}