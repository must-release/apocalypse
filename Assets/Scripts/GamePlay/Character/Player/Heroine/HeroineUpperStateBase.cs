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

    public virtual void Move(HorizontalDirection horizontalInput) { /* Do not call base method in derived method */ }
    public virtual void LookUp(bool lookUp) { /* Do not call base method in derived method */ }
    public virtual void Jump() { /* Do not call base method in derived method */ }
    public virtual void Enable() { /* Do not call base method in derived method */ }
    public virtual void Disable() { /* Do not call base method in derived method */ }
    public virtual void Aim(Vector3 aim) { /* Do not call base method in derived method */ }


    /****** Private Members ******/

    private ILowerBodyStateInfo _lowerBodyStateInfo;
}
