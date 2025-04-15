using System;
using UnityEngine;
using EventEnums;


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

    public bool IsFromAddressables
    {
        get => _isFromAddressables;
        protected set => _isFromAddressables = value;
    }

    /****** Protected Members ******/

    // Called when script is loaded
    protected abstract void OnEnable();

    // Called when property is changed by the inspector
    protected abstract void OnValidate();


    /****** Private Members ******/

    [SerializeField, HideInInspector] private GameEventType      _eventType       = GameEventType.GameEventTypeCount;
    [SerializeField, HideInInspector] private bool               _isInitialized   = false;
    
    private bool _isFromAddressables = true;
}