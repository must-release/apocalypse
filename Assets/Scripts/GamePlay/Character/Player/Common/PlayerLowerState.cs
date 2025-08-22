using UnityEngine;

public abstract class PlayerLowerState : PlayerStateBase<LowerStateType>, IPlayerLowerState
{
    /****** Public Members ******/
    public virtual bool ShouldDisableUpperBody => false;
    public float AnimationNormalizedTime => StateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

    public virtual void OnEnter()
    {
        // Do not call base.OnEnter() in overriding methods.
    }

    public virtual void OnUpdate()
    {
        // Do not call base.OnUpdate() in overriding methods.
    }

    public virtual void OnExit(LowerStateType nextState)
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

    public virtual void OnGround()
    {
        // Do not call base.OnGround() in overriding methods.
    }

    public virtual void OnDamaged()
    {
        // Do not call base.OnDamaged() in overriding methods.
    }

    public virtual void StartJump()
    {
        // Do not call base.StartJump() in overriding methods.
    }

    public virtual void CheckJumping(bool isJumping)
    {
        // Do not call base.CheckJumping() in overriding methods.
    }

    public virtual void Aim(Vector3 aim)
    {
        // Do not call base.Aim() in overriding methods.
    }

    public virtual void Move(HorizontalDirection horizontalInput)
    {
        // Do not call base.Move() in overriding methods.
    }

    public virtual void Tag()
    {
        // Do not call base.Tag() in overriding methods.
    }

    public virtual void Push(bool push)
    {
        // Do not call base.Push() in overriding methods.
    }

    public virtual void UpDown(VerticalDirection verticalDirection)
    {
        // Do not call base.UpDown() in overriding methods.
    }

    public virtual void Attack()
    {
        // Do not call base.Attack() in overriding methods.
    }


    /****** Protected Members ******/
    protected static float _jumpBufferTimer; // Timer for jump buffering
    protected const float JUMP_BUFFER_DURATION = 0.4f; // Duration for jump buffering
}