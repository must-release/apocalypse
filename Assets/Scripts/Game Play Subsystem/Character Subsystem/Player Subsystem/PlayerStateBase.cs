using CharacterEnums;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class PlayerLowerStateBase<TStateEnum> : MonoBehaviour where TStateEnum : System.Enum
{
    /****** Public Members ******/

    public abstract TStateEnum StateType { get; }
    public abstract bool ShouldDisableUpperBody { get; }
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();

    public virtual void InitializeState(IStateController<TStateEnum> stateController, IMotionController playerPhysics, ICharacterInfo playerInfo)
    {
        Assert.IsTrue(null != stateController, "StateController is not assigned.");
        Assert.IsTrue(null != playerPhysics, "PlayerPhysics is not assigned.");
        Assert.IsTrue(null != playerInfo, "PlayerInfo is not assigned.");

        StateController = stateController;
        PlayerMotion   = playerPhysics;
        PlayerInfo      = playerInfo;
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

    protected IStateController<TStateEnum>  StateController { get; private set; }
    protected IMotionController             PlayerMotion   { get; private set; }
    protected ICharacterInfo                PlayerInfo      { get; private set; }
}


public abstract class PlayerUpperStateBase<TStateEnum> : MonoBehaviour where TStateEnum : System.Enum
{
    /****** Public Members ******/

    public abstract TStateEnum  StateType { get; }
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(TStateEnum nextState);

    public virtual void InitializeState(IStateController<TStateEnum> stateController, IMotionController playerPhysics, ICharacterInfo playerInfo)
    {
        Assert.IsTrue(null != stateController, "StateController is not assigned.");
        Assert.IsTrue(null != playerPhysics, "PlayerPhysics is not assigned.");
        Assert.IsTrue(null != playerInfo, "PlayerInfo is not assigned.");

        StateController = stateController;
        PlayerMotion   = playerPhysics;
        PlayerInfo      = playerInfo;
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
    
    protected IStateController<TStateEnum>  StateController { get; private set; }
    protected IMotionController             PlayerMotion    { get; private set; }
    protected ICharacterInfo                PlayerInfo      { get; private set; }
}