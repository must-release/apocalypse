using UnityEngine;

public abstract class HeroineLowerStateBase : PlayerStateBase<HeroineLowerState>, ILowerBodyStateInfo
{
    /****** Public Members ******/

    public float AnimationNormalizedTime => StateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;

    public abstract bool ShouldDisableUpperBody { get; }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(HeroineLowerState nextState);


    public virtual void OnFixedUpdate() { }
    public virtual void StartJump() { }
    public virtual void CheckJumping(bool isJumping) { }
    public virtual void OnAir() { }
    public virtual void Aim(Vector3 aim) { }
    public virtual void Attack() { }
    public virtual void Move(HorizontalDirection horizontalInput) { }
    public virtual void Tag() { }
    public virtual void Climb(bool climb) { }
    public virtual void OnGround() { }
    public virtual void Push(bool push) { }
    public virtual void UpDown(VerticalDirection verticalDirection ) { }
    public virtual void Damaged() { }
}