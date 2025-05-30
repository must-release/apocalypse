using System;
using UnityEngine;

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

    public void DestroyInfo()
    {
        if (_isRuntimeResource)
            Destroy(this);
    }

    public abstract GameEventInfo Clone();
    public abstract GameEventDTO ToDTO();

    /****** Protected Members ******/

    protected bool IsRuntimeInstance
    {
        set => _isRuntimeResource = value;
    }

    // Called when script is loaded
    protected abstract void OnEnable();

    // Called when property is changed by the inspector
    protected abstract void OnValidate();


    /****** Private Members ******/

    [SerializeField, HideInInspector] private GameEventType      _eventType       = GameEventType.GameEventTypeCount;
    [SerializeField, HideInInspector] private bool               _isInitialized   = false;

    private bool _isRuntimeResource = false;
}