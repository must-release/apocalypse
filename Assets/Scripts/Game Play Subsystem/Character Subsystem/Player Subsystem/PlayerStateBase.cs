using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class PlayerLowerStateBase<TStateEnum> : MonoBehaviour, IAsyncLoadObject where TStateEnum : System.Enum
{
    /****** Public Members ******/

    public bool IsLoaded => _isClipLoaded;

    public abstract TStateEnum StateType { get; }
    public abstract bool ShouldDisableUpperBody { get; }
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();

    public virtual void InitializeState(IStateController<TStateEnum> stateController, IMotionController playerMotion, ICharacterInfo playerInfo, Animator lowerAnimator)
    {
        Assert.IsTrue(null != stateController, "StateController is not assigned.");
        Assert.IsTrue(null != playerMotion, "PlayerMotion is not assigned.");
        Assert.IsTrue(null != playerInfo, "PlayerInfo is not assigned.");
        Assert.IsTrue(null != lowerAnimator, "LowerAnimator is not assigned.");

        StateController = stateController;
        PlayerMotion    = playerMotion;
        PlayerInfo      = playerInfo;
        LowerAnimator   = lowerAnimator;

        StartCoroutine(AsyncLoadAnimationClip());
    }

    public virtual void Jump() {}
    public virtual void OnAir() {}
    public virtual void Aim(bool isAiming) {}
    public virtual void Move(int move) {}
    public virtual void Tag() {}
    public virtual void Climb(bool climb) {}
    public virtual void OnGround() {}
    public virtual void Stop() {}
    public virtual void Push(bool push) {}
    public virtual void UpDown(int upDown) {}
    public virtual void Damaged() {}


    /****** Protected Members ******/

    protected IStateController<TStateEnum>  StateController     { get; private set; } = null;
    protected IMotionController             PlayerMotion        { get; private set; } = null;
    protected ICharacterInfo                PlayerInfo          { get; private set; } = null;
    protected Animator                      LowerAnimator       { get; private set; } = null;
    protected AnimationClip                 LowerAnimationClip  { get; private set; } = null;

    protected virtual string AnimationClipPath => null;


    /****** Private Members ******/

    private bool _isClipLoaded = false;

    private IEnumerator AsyncLoadAnimationClip()
    {
        Assert.IsTrue(null != AnimationClipPath, "Animation Clip Path is not set.");


        var handle = Addressables.LoadAssetAsync<AnimationClip>(AnimationClipPath);
        yield return handle;

        if (AsyncOperationStatus.Succeeded == handle.Status)
        {
            Debug.Log("Loaded lower animation clip : " +  AnimationClipPath);

            LowerAnimationClip  = handle.Result;
            _isClipLoaded       = true;
        }
        else
        {
            Debug.LogError("Failed to load lower animation clip : " + AnimationClipPath);
        }
    }
}


public abstract class PlayerUpperStateBase<TStateEnum> : MonoBehaviour where TStateEnum : System.Enum
{
    /****** Public Members ******/

    public bool IsLoaded => _isClipLoaded;

    public abstract TStateEnum  StateType { get; }
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(TStateEnum nextState);

    public virtual void InitializeState(IStateController<TStateEnum> stateController, IMotionController playerMotion, ICharacterInfo playerInfo, Animator upperAnimator)
    {
        Assert.IsTrue(null != stateController, "StateController is not assigned.");
        Assert.IsTrue(null != playerMotion, "PlayerMotion is not assigned.");
        Assert.IsTrue(null != playerInfo, "PlayerInfo is not assigned.");
        Assert.IsTrue(null != upperAnimator, "UpperAnimator is not assigned.");

        StateController = stateController;
        PlayerMotion    = playerMotion;
        PlayerInfo      = playerInfo;
        UpperAnimator   = upperAnimator;

        StartCoroutine(AsyncLoadAnimationClip());
    }

    public virtual void Move() {}
    public virtual void OnAir() {}
    public virtual void LookUp(bool lookUp) {}
    public virtual void Attack() {}
    public virtual void Jump() {}
    public virtual void Stop() {}
    public virtual void OnGround() {}
    public virtual void Enable() {}
    public virtual void Disable() {}
    public virtual void Aim(Vector3 aim) {}

    /****** Protected Members ******/

    protected IStateController<TStateEnum>  StateController     { get; private set; } = null;
    protected IMotionController             PlayerMotion        { get; private set; } = null;
    protected ICharacterInfo                PlayerInfo          { get; private set; } = null;
    protected Animator                      UpperAnimator       { get; private set; } = null;
    protected AnimationClip                 UpperAnimationClip  { get; private set; } = null;

    protected virtual string AnimationClipPath => null;


    /****** Private Members ******/

    private bool _isClipLoaded = false;

    private IEnumerator AsyncLoadAnimationClip()
    {
        Assert.IsTrue(null != AnimationClipPath, "Animation Clip Path is not set.");


        var handle = Addressables.LoadAssetAsync<AnimationClip>(AnimationClipPath);
        yield return handle;

        if (AsyncOperationStatus.Succeeded == handle.Status)
        {
            Debug.Log("Loaded upper animation clip : " + AnimationClipPath);

            UpperAnimationClip  = handle.Result;
            _isClipLoaded       = true;
        }
        else
        {
            Debug.LogError("Failed to load upper animation clip : " + AnimationClipPath);
        }
    }
}