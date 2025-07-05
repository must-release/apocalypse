using UnityEngine;

public abstract class HeroUpperStateBase : IPlayerUpperState
{
    /****** Public Members ******/

    public virtual void Move(HorizontalDirection horizontalInput) { }
    public virtual void Attack() { }
    public virtual void LookUp(bool lookUp) { }
    public virtual void Jump() { }
    public virtual void OnAir() { }
    public virtual void Stop() { }
    public virtual void Aim(Vector3 aim) { }
}
