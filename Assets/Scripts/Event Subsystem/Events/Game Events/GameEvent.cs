using System;
using UnityEngine;
using EventEnums;
using System.Collections.Generic;
using UnityEngine.Assertions;

public abstract class GameEvent : MonoBehaviour
{
    /******* Public Members ******/

    public EventStatus Status { get; protected set; } = EventStatus.EventStatusCount;
    public Action OnTerminate;
    
    public abstract bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts);
    public abstract GameEventInfo GetEventInfo();
    public abstract GameEventType GetEventType();
    public abstract bool ShouldBeSaved();

    public virtual void PlayEvent()
    {
        Assert.IsTrue(EventStatus.Waiting == Status, "Event status must be set as waiting before execution.");

        Status = EventStatus.Running;
    }

    public virtual void TerminateEvent()
    {
        Assert.IsTrue(EventStatus.Terminated != Status, "Event is already terminated.");
        Assert.IsTrue(OnTerminate != null, "Terminate handler must be set.");

        Status = EventStatus.Terminated;
        OnTerminate.Invoke();
    }
}