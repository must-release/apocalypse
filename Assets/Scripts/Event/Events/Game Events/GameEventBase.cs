using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public abstract class GameEventBase<TEventInfo> : MonoBehaviour, IGameEvent where TEventInfo : GameEventInfo
{
    /******* Public Members ******/

    public EventStatus      Status              { get; protected set; } = EventStatus.EventStatusCount;
    public IEventContainer  EventContainer
    {
        get => _eventContainer;
        set
        {
            Assert.IsTrue(value.IsContainingEvent(this), "The event container is not containing this event.");
            _eventContainer = value;
        }
    }
    public Action           OnTerminate         { get; set; }
    public GameEventInfo    EventInfo           => Info;
    public int              EventId             => GetInstanceID();
    public virtual bool     IsExclusiveEvent    => false; // By default, events are not exclusive.

    public abstract GameEventType   EventType       { get; }
    public abstract bool            ShouldBeSaved   { get; }
    
    public abstract bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts);

    public virtual void Initialize(TEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        Info    = eventInfo;
        Status  = EventStatus.Waiting;

        OnTerminate = null;
    }

    public virtual void UpdateStatus() { }

    public virtual void PlayEvent()
    {
        Assert.IsTrue(EventStatus.Waiting == Status, "Event status must be set as waiting before execution.");

        Status = EventStatus.Running;
    }

    public virtual void TerminateEvent()
    {
        Assert.IsTrue(EventStatus.Terminated != Status, $"The {EventType} event is already terminated.");
        Assert.IsTrue(OnTerminate != null, $"Terminate handler for the {EventType} event must be set.");

        Status = EventStatus.Terminated;
        OnTerminate.Invoke();

        Logger.Write(LogCategory.Event, $"Terminating Event : {EventType}, Instance : {EventId}", LogLevel.Log, true);
    }


    /****** Protected Members ******/

    protected TEventInfo Info { get; set; }


    /****** Private Members ******/

    IEventContainer _eventContainer = null; 
}