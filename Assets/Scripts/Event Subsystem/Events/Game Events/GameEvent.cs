using System;
using UnityEngine;
using EventEnums;
using UIEnums;
using System.Collections;

public abstract class GameEvent : ScriptableObject
{
    /******* Public Members ******/

    public GameEventInfo GameEventInfo { get; }
    public GameEventType EventType { get { return (GameEventType)_eventType; } set { _eventType = (int)value; } }
    public GameEvent NextEvent { get { return _nextEvent; } set { _nextEvent = value; } }
    public GameEvent ParentEvent { get { return _parentEvent; } set { _parentEvent = value; } }

    // Check compatibiliry with parent event and current UI
    public abstract bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI);

    public abstract void PlayEvent();

    public virtual IEnumerator PlayEventCoroutine() { return default; }

    public virtual void TerminateEvent() { return; }

    // Save flawless info of the nextEvent
    public void SaveNextEventInfo()
    {
        if ( null == _nextEvent )
            return;

        // Save type of the nextEvent
        _nextEventAssemblyQualifiedName = _nextEvent.GetType().AssemblyQualifiedName;
        // Save Json data of the nextEvent
        _nextEventdata = JsonUtility.ToJson(_nextEvent);
    }

    // Restore data of the nextEvent
    public void RestoreNextEventInfo()
    {
        if ( string.IsNullOrEmpty(_nextEventAssemblyQualifiedName) || string.IsNullOrEmpty(_nextEventdata) )
            return;

        // Create objects and deserialize data based on stored type information
        Type eventType = Type.GetType(_nextEventAssemblyQualifiedName);
        GameEvent eventInstance = (GameEvent)CreateInstance(eventType);
        JsonUtility.FromJsonOverwrite(_nextEventdata, eventInstance);
        _nextEvent = eventInstance;
        _nextEvent.RestoreNextEventInfo();
    }


    /****** Private Memebers ******/

    private GameEventInfo _gameEventInfo;
    private int _eventType;
    private GameEvent _nextEvent;
    private GameEvent _parentEvent;
    [SerializeField] private string _nextEventAssemblyQualifiedName; 
    [SerializeField] private string _nextEventdata;
}


[Serializable]
public abstract class GameEventInfo
{
    /****** Public Members ******/

    public GameEventType GameEventType 
    { 
        get { return (GameEventType)_gameEventType; } 
        set 
        { 
            _gameEventType = (int)value; 
        }
    }

    public GameEventInfo( GameEventType gameEventType = GameEventType.GameEventTypeCount )
    {
        GameEventType = gameEventType;
    }


    /****** Private Members ******/

    private int _gameEventType;
}