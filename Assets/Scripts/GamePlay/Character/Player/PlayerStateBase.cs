using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class PlayerStateBase<TStateEnum> : MonoBehaviour where TStateEnum : System.Enum
{
    /****** Public Members ******/

    public abstract TStateEnum StateType { get; }

    public virtual void InitializeState(IStateController<TStateEnum> stateController,
                                        IMotionController playerMotion,
                                        ICharacterInfo playerInfo,
                                        Animator stateAnimator,
                                        PlayerWeaponBase playerWeapon
    )
    {
        Assert.IsTrue(null != stateController, $"StateController in {StateType} is not assigned.");
        Assert.IsTrue(null != playerMotion, $"PlayerMotion in {StateType} is not assigned.");
        Assert.IsTrue(null != playerInfo, $"PlayerInfo in {StateType} is not assigned.");
        Assert.IsTrue(null != stateAnimator, $"StateAnimator in {StateType} is not assigned.");
        Assert.IsTrue(null != playerWeapon, $"PlayerWeapon in {StateType} is not assigned.");

        StateController = stateController;
        PlayerMotion    = playerMotion;
        PlayerInfo      = playerInfo;
        PlayerWeapon    = playerWeapon;
        StateAnimator   = stateAnimator;
    }


    /****** Protected Members ******/

    protected IStateController<TStateEnum>  StateController     { get; private set; } = null;
    protected IMotionController             PlayerMotion        { get; private set; } = null;
    protected ICharacterInfo                PlayerInfo          { get; private set; } = null;
    protected PlayerWeaponBase              PlayerWeapon        { get; private set; } = null;
    protected Animator                      StateAnimator       { get; private set; } = null;
}