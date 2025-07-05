public interface IPlayerUpperState
{
    ILowerStateInfo LowerStateInfo { get; set; }
    void OnEnter();
    void OnUpdate();
    void OnExit(UpperStateType nextState);
    void OnFixedUpdate();
    void Enable();
    void Disable();
}