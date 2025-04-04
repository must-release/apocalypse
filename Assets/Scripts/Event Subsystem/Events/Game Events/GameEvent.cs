using System;
using UnityEngine;
using EventEnums;
using UIEnums;
using System.Collections.Generic;
using UnityEngine.Assertions;

public abstract class GameEvent : MonoBehaviour
{
    /******* Public Members ******/

    public GameEventType EventType => GetEventInfo().EventType;
    public EventStatus Status { get; protected set; } = EventStatus.EventStatusCount;
    public System.Action OnTerminate;
    
    public abstract bool CheckCompatibility();
    public abstract GameEventInfo GetEventInfo();

    public virtual void PlayEvent()
    {
        Assert.IsTrue(EventStatus.Waiting == Status, "Event status must be set as waiting before execution.");

        Status = EventStatus.Running;
    }

    protected virtual void TerminateEvent()
    {
        Assert.IsTrue(EventStatus.Terminated != Status, "Event is already terminated.");
        Assert.IsTrue(OnTerminate != null, "Terminate handler must be set.");

        Status = EventStatus.Terminated;
        OnTerminate.Invoke();
    }
}


[Serializable]
public abstract class GameEventInfo : ScriptableObject
{
    /****** Public Members ******/

    public GameEventType EventType
    {
        get => _eventType;
        protected set => _eventType = value;
    }

    public bool IsInitialized
    {
        get => _isInitialized;
        protected set => _isInitialized = value;
    }

    /****** Protected Members ******/

    // Called when script is loaded
    protected abstract void OnEnable();

    // Called when property is changed by the inspector
    protected abstract void OnValidate();


    /****** Private Members ******/

    private GameEventType       _eventType       = GameEventType.GameEventTypeCount;
    private bool                _isInitialized   = false;
}