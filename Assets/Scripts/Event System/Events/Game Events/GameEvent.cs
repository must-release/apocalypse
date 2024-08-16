using System;
using UnityEngine;
using EventEnums;
using UIEnums;
using System.Collections;

[Serializable]
public class GameEvent : ScriptableObject
{
    /* Event info which is used when playing game */
    [HideInInspector]
    public int eventType;
    public EVENT_TYPE EventType { get { return (EVENT_TYPE)eventType; } set { eventType = (int)value; } }
    public GameEvent nextEvent;
    public GameEvent NextEvent { get { return nextEvent; } set { nextEvent = value; } }
    public GameEvent parentEvent;
    public GameEvent ParentEvent { get { return parentEvent; } set { parentEvent = value; } }

    /* Event info which is used when saving data */
    [SerializeField, HideInInspector]
    private string nextEventAssemblyQualifiedName; 
    [SerializeField, HideInInspector]
    private string nextEventdata;

    // Check compatibiliry with parent event and current UI
    public virtual bool CheckCompatibility(GameEvent parentEvent, BASEUI baseUI, SUBUI subUI)
    {
        return default;
    }

    // Play this game event
    public virtual void PlayEvent()
    {
        return;
    }

    public virtual IEnumerator PlayEventCoroutine()
    {
        return default;
    }

    // Terminate this game event
    public virtual void TerminateEvent()
    {
        return;
    }

    // Save flawless info of the nextEvent
    public void SaveNextEventInfo()
    {
        if (nextEvent != null)
        {
            // Save type of the nextEvent
            nextEventAssemblyQualifiedName = nextEvent.GetType().AssemblyQualifiedName;
            // Save Json data of the nextEvent
            nextEventdata = JsonUtility.ToJson(nextEvent);
        }
    }

    // Restore data of the nextEvent
    public void RestoreNextEventInfo()
    {
        if (!string.IsNullOrEmpty(nextEventAssemblyQualifiedName) && !string.IsNullOrEmpty(nextEventdata))
        {
            // Create objects and deserialize data based on stored type information
            Type eventType = Type.GetType(nextEventAssemblyQualifiedName);
            GameEvent eventInstance = (GameEvent)CreateInstance(eventType);
            JsonUtility.FromJsonOverwrite(nextEventdata, eventInstance);
            nextEvent = eventInstance;
            nextEvent.RestoreNextEventInfo();
        }
    }
}