using UnityEngine;

public interface IPlayerLowerState : IPlayerState<LowerStateType>, ILowerStateInfo
{
    bool ShouldDisableUpperBody { get; }
    void OnEnter();
    void OnUpdate();
    void OnExit(LowerStateType nextState);
    void OnFixedUpdate();
    void OnAir();
    void OnGround();
    void OnDamaged();
    void StartJump();
    void CheckJumping(bool isJumping);
    void Aim(Vector3 aim);
    void Move(HorizontalDirection horizontalInput);
    void Attack();
    void Tag();
    void Push(bool push);
    void UpDown(VerticalDirection verticalDirection);
}