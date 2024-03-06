using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class UserData : ISerializationCallbackReceiver
{
    public enum STAGE { TEST, TUTORIAL, LIBRARY }
    public enum CHARACTER { HERO, HEROINE }

    public STAGE currentStage;
    public int currentMap;
    public int lastDialogueNum;
    public CHARACTER lastChar;
    public string playTime;
    public string saveTime;

    // Used for serialization
    [SerializeField]
    private string startingEventAssemblyQualifiedName;
    [SerializeField]
    private string startingEventData;

    // Used for game
    [System.NonSerialized]
    public IEvent startingEvent;
    public IEvent StartingEvent { get { return startingEvent; } set { startingEvent = value; } }


    public UserData(STAGE curStage, int curMap, IEvent startingEvent, int lastDlg, CHARACTER lastChar, string playTime, string saveTime)
    {
        currentStage = curStage;
        currentMap = curMap;
        StartingEvent = startingEvent;
        lastDialogueNum = lastDlg;
        this.lastChar = lastChar;
        this.playTime = playTime;
        this.saveTime = saveTime;
    }

    public UserData Copy()
    {
        return new UserData(currentStage, currentMap, StartingEvent, lastDialogueNum, lastChar, playTime, saveTime);
    }

    // Save info of the startingEvent
    public void OnBeforeSerialize()
    {
        if (startingEvent != null)
        {
            // Get all chained events
            Stack<IEvent> eventStack = new Stack<IEvent>();
            IEvent nextEvent = startingEvent.NextEvent;
            while (nextEvent != null)
            {
                eventStack.Push(nextEvent);
                nextEvent = nextEvent.NextEvent;
            }

            // Create string Json data for all events
            while (eventStack.Count > 0)
            {
                IEvent @event = eventStack.Pop();
                @event.OnBeforeSerialize();
            }

            // Create Json data of StartingEvent
            startingEventAssemblyQualifiedName = startingEvent.GetType().AssemblyQualifiedName;
            startingEventData = JsonUtility.ToJson(startingEvent);
        }
    }

    // Restore data of the startingEvent
    public void OnAfterDeserialize()
    {
        if (!string.IsNullOrEmpty(startingEventAssemblyQualifiedName) && !string.IsNullOrEmpty(startingEventData))
        {
            // Restore Starting event
            Type eventType = Type.GetType(startingEventAssemblyQualifiedName);
            IEvent eventInstance = (IEvent)ScriptableObject.CreateInstance(eventType);
            JsonUtility.FromJsonOverwrite(startingEventData, eventInstance);
            startingEvent = eventInstance;
        }
    }
}