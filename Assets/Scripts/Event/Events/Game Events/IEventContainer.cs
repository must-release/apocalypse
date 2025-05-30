using UnityEngine;

public interface IEventContainer : IGameEvent
{
    void AddNewEvent(IGameEvent newEvent);
    bool IsContainingEvent(IGameEvent findingEvent);
}
