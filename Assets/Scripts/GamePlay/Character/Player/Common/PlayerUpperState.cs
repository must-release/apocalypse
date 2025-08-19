using UnityEngine;

public abstract class PlayerUpperState : PlayerStateBase<UpperStateType>, IPlayerUpperState
{
    /****** Public Members ******/
    public ILowerStateInfo LowerStateInfo { get; set; }

    public virtual void OnEnter()
    {
        // Do not call base.OnEnter() in overriding methods.
    }

    public virtual void OnUpdate()
    {
        // Do not call base.OnUpdate() in overriding methods.
    }

    public virtual void OnExit(UpperStateType nextState)
    {
        // Do not call base.OnExit() in overriding methods.
    }

    public virtual void OnFixedUpdate()
    {
        // Do not call base.OnFixedUpdate() in overriding methods.
    }

    public virtual void OnAir()
    {
        // Do not call base.OnAir() in overriding methods.
    }

    public virtual void Enable()
    {
        // Do not call base.Enable() in overriding methods.
    }

    public virtual void Disable()
    {
        // Do not call base.Disable() in overriding methods.
    }

    public virtual void Move(HorizontalDirection horizontalInput)
    {
        // Do not call base.Move() in overriding methods.
    }

    public virtual void LookUp(bool lookUp)
    {
        // Do not call base.LookUp() in overriding methods.
    }

    public virtual void Jump()
    {
        // Do not call base.Jump() in overriding methods.
    }

    public virtual void Aim(Vector3 aim)
    {
        // Do not call base.Aim() in overriding methods.
    }

    public virtual void Attack()
    {
        // Do not call base.Attack() in overriding methods.
    }

    public virtual void OnGround()
    {
        // Do not call base.OnGround() in overriding methods.
    }
}
