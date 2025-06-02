using UnityEngine;

public interface IStateController<TStateEnum> where TStateEnum : System.Enum
{
    void ChangeState(TStateEnum newState);
}
