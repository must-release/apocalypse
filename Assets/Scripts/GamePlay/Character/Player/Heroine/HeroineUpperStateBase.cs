using UnityEngine;

public abstract class HeroineUpperStateBase : PlayerStateBase<HeroineUpperState>
{
    /****** Public Members ******/

    public ILowerBodyStateInfo LowerBodyStateInfo
    {
        protected get => _lowerBodyStateInfo;
        set => _lowerBodyStateInfo = value;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(HeroineUpperState nextState);

    public virtual void Move() { }
    public virtual void LookUp(bool lookUp) { }
    public virtual void Jump() { }
    public virtual void Stop() { }
    public virtual void Enable() { }
    public virtual void Disable() { }
    public virtual void Aim(Vector3 aim) { }


    /****** Private Members ******/

    private ILowerBodyStateInfo _lowerBodyStateInfo;
}
