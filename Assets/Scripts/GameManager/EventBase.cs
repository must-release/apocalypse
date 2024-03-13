using System;
using UnityEngine;

[System.Serializable]
public class EventBase : ScriptableObject, ISerializationCallbackReceiver
{
    public enum TYPE { STORY, TUTORIAL, IN_GAME, MAP_CHANGE, LOADING, AUTO_SAVE };

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


    // Initalize variables
    public void Initialize(EventBase nextEvent)
    {
        NextEvent = nextEvent;
    }

    // Save flawless info of the nextEvent
    public void OnBeforeSerialize()
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
    public void OnAfterDeserialize()
    {
        if (!string.IsNullOrEmpty(nextEventAssemblyQualifiedName) && !string.IsNullOrEmpty(nextEventdata))
        {
            // Create objects and deserialize data based on stored type information
            Type eventType = Type.GetType(nextEventAssemblyQualifiedName);
            EventBase eventInstance = (EventBase)CreateInstance(eventType);
            JsonUtility.FromJsonOverwrite(nextEventdata, eventInstance);
            nextEvent = eventInstance;
        }
    }
}