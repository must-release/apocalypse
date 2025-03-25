using System;
using UnityEngine;
using EventEnums;
using UIEnums;
using System.Collections;

public abstract class GameEvent : MonoBehaviour
{
    /******* Public Members ******/

    public GameEventType    EventType { get { return (GameEventType)_eventType; } set { _eventType = (int)value; } }
    public GameEvent        NextEvent { get { return _nextEvent; } set { _nextEvent = value; } }
    public GameEvent        ParentEvent { get { return _parentEvent; } set { _parentEvent = value; } }

    // Check compatibiliry with parent event and current UI
    public abstract bool            CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI);
    public abstract void            PlayEvent(GameEventInfo evebtInfo);
    public abstract void            TerminateEvent();
    public abstract GameEventInfo   GetEventInfo();

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

        // // Create objects and deserialize data based on stored type information
        // Type eventType = Type.GetType(_nextEventAssemblyQualifiedName);
        // GameEvent eventInstance = (GameEvent)CreateInstance(eventType);
        // JsonUtility.FromJsonOverwrite(_nextEventdata, eventInstance);
        // _nextEvent = eventInstance;
        // _nextEvent.RestoreNextEventInfo();
    }


    /****** Private Memebers ******/

    private int             _eventType;
    private GameEvent       _nextEvent;
    private GameEvent       _parentEvent;

    [SerializeField] private string _nextEventAssemblyQualifiedName; 
    [SerializeField] private string _nextEventdata;
}


[Serializable]
public abstract class GameEventInfo : ScriptableObject
{
    /****** Public Members ******/

    public GameEventType    EventType { get { return _eventType; } protected set { _eventType = value; } }
    public bool             IsInitialized { get { return _isInitialized; } protected set { _isInitialized = value; }}


    /****** Protected Members ******/

    // Called when script is loaded
    protected abstract void OnEnable();

    // Called when property is changed by the inspector
    protected abstract void OnValidate();


    /****** Private Members ******/

    private GameEventType   _eventType;
    private bool            _isInitialized = false;
}


[Serializable]
public class GameEventList : ScriptableObject
{
    /****** Public Members ******/



    /****** Protected Members ******/



    /******* Private Members ******/
}