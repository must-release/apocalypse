using UnityEngine;

public interface IStateController<TStateEnum> where TStateEnum : System.Enum
{
    /// <summary>
    /// Change the state of the controller.
    /// </summary>
    /// <param name="newState">change current state to newState</param>
    void ChangeState(TStateEnum newState);
}
