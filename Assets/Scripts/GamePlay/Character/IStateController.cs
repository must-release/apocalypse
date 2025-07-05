public interface IStateController<in TState> where TState : StateType
{
    void ChangeState(TState newState);
}

public interface ILowerStateController : IStateController<LowerStateType>
{
    /* empty */
}

public interface IUpperStateController : IStateController<UpperStateType>
{
    /* empty */
}