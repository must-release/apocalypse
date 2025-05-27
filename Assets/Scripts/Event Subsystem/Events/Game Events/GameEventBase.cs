using System;
using UnityEngine;
using EventEnums;
using System.Collections.Generic;
using UnityEngine.Assertions;

public abstract class GameEventBase<TEventInfo> : MonoBehaviour, IGameEvent where TEventInfo : GameEventInfo
{
    /******* Public Members ******/

    public EventStatus  Status          { get; protected set; } = EventStatus.EventStatusCount;
    public IGameEvent   ParentEvent     { get; protected set; } = null;
    public Action       OnTerminate     { get; set; }

    public abstract GameEventInfo   EventInfo       { get; }
    public abstract GameEventType   EventType       { get; }
    public abstract bool            ShouldBeSaved   { get; }


    public abstract void Initialize(TEventInfo eventInfo, IGameEvent parentEvent = null);
    
    public abstract bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts);

    public virtual void UpdateStatus() { }

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