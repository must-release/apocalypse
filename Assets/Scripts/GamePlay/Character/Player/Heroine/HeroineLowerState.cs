using UnityEngine;

public abstract class HeroineLowerState : IPlayerLowerState
{
    /****** Public Members ******/
    public virtual void StartJump() { }
    public virtual void CheckJumping(bool isJumping) { }
    public virtual void Aim(Vector3 aim) { }
    public virtual void Attack() { }
    public virtual void Move(HorizontalDirection horizontalInput) { }
    public virtual void Tag() { }
    public virtual void Climb(bool climb) { }
    public virtual void Push(bool push) { }
    public virtual void UpDown(VerticalDirection verticalDirection) { }
}