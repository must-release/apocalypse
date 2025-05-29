using EventEnums;
using System.Collections.Generic;
using System;

public interface IGameEvent
{
    EventStatus     Status              { get; }
    IEventContainer EventContainer      { get; set; }
    GameEventInfo   EventInfo           { get; }
    GameEventType   EventType           { get; }
    Action          OnTerminate         { get; set; }
    int             EventId             { get; }
    bool            ShouldBeSaved       { get; }
    bool            IsExclusiveEvent    { get; } // If true, this event cannot be run with other events except the container event of this event.

    bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts);
    void PlayEvent();
    void TerminateEvent();
    void UpdateStatus();
}
