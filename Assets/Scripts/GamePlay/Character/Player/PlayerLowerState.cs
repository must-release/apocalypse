public interface IPlayerLowerState : ILowerStateInfo
{
    bool ShouldDisableUpperBody { get; }
    void OnEnter();
    void OnUpdate();
    void OnExit(LowerStateType nextState);
    void OnFixedUpdate();
    void OnAir();
    void OnGround();
    void OnDamaged();
}