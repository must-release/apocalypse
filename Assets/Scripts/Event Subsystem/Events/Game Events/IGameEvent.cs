using EventEnums;
using System.Collections.Generic;
using System;

public interface IGameEvent
{
    EventStatus     Status      { get; }
    IGameEvent      ParentEvent { get; }
    GameEventInfo   EventInfo { get; }
    GameEventType   EventType { get; }
    Action          OnTerminate { get; set; }
    bool            ShouldBeSaved { get; }

    bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts);
    void PlayEvent();
    void TerminateEvent();
    void UpdateStatus();
}
