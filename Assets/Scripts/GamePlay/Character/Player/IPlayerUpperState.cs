using UnityEngine;

public interface IPlayerUpperState : IPlayerState<UpperStateType>
{
    ILowerStateInfo LowerStateInfo { get; set; }
    void OnEnter();
    void OnUpdate();
    void OnExit(UpperStateType nextState);
    void OnFixedUpdate();
    void OnAir();
    void OnGround();
    void Enable();
    void Disable();
    void Move(HorizontalDirection horizontalInput);
    void LookUp(bool lookUp);
    void Jump();
    void Aim(Vector3 aim);
    void Attack();
}