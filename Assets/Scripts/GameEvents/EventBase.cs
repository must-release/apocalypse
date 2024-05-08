using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EventBase : ScriptableObject
{
    public enum TYPE { STORY, TUTORIAL, IN_GAME, MAP_CHANGE, LOADING, DATA_SAVE, UI_CHANGE, DATA_LOAD, SCENE_LOAD };

    /* Event info which is used when playing game */
    [HideInInspector]
    public int eventType;
    public TYPE EventType { get { return (TYPE)eventType; } set { eventType = (int)value; } }
    public EventBase nextEvent;
    public EventBase NextEvent { get { return nextEvent; } set { nextEvent = value; } }


    /* Event info which is used when saving data */
    [SerializeField, HideInInspector]
    private string nextEventAssemblyQualifiedName; 
    [SerializeField, HideInInspector]
    private string nextEventdata;

    // Return important info of the event
    public virtual T GetEventInfo<T>()
    {
        return default;
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
            EventBase eventInstance = (EventBase)CreateInstance(eventType);
            JsonUtility.FromJsonOverwrite(nextEventdata, eventInstance);
            nextEvent = eventInstance;
            nextEvent.RestoreNextEventInfo();
        }
    }
}