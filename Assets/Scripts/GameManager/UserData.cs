using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class UserData : ISerializationCallbackReceiver
{
    public enum STAGE { TEST, TUTORIAL, LIBRARY }
    public enum CHARACTER { HERO, HEROINE }

    [SerializeField]
    private int currentStage;
    public STAGE CurrentStage { get { return (STAGE)currentMap; } set { currentStage = (int)value; } }

    [SerializeField]
    private int currentMap;
    public int CurrentMap { get { return currentMap; } set { currentMap = value; } }

    [SerializeField]
    private int lastDialogueNumber;
    public int LastDialogueNumber { get { return lastDialogueNumber; } set { lastDialogueNumber = value; } }

    private int lastCharacter;
    public CHARACTER LastCharacter { get { return (CHARACTER)lastCharacter; } set { lastCharacter = (int)value; } }

    [SerializeField]
    private string playTime;
    public string PlayTime { get { return playTime; } set { playTime = value; } }

    [SerializeField]
    private string saveTime;
    public string SaveTime { get { return saveTime; } set { saveTime = value; } }

    // Used for game
    [System.NonSerialized]
    public EventBase startingEvent;
    public EventBase StartingEvent { get { return startingEvent; } set { startingEvent = value; } }

    // Used for serialization
    [SerializeField]
    private string startingEventAssemblyQualifiedName;
    [SerializeField]
    private string startingEventData;


    public UserData(STAGE curStage, int curMap, EventBase startingEvent, int lastDlg, CHARACTER lastChar, string playTime, string saveTime)
    {
        CurrentStage = curStage;
        CurrentMap = curMap;
        StartingEvent = startingEvent;
        LastDialogueNumber = lastDlg;
        LastCharacter = lastChar;
        PlayTime = playTime;
        SaveTime = saveTime;
    }

    public UserData Copy()
    {
        return new UserData(CurrentStage, CurrentMap, StartingEvent, LastDialogueNumber, LastCharacter, PlayTime, SaveTime);
    }

    // Save info of the startingEvent
    public void OnBeforeSerialize()
    {
        if (StartingEvent != null)
        {
            // Create Json data of StartingEvent
            startingEventAssemblyQualifiedName = StartingEvent.GetType().AssemblyQualifiedName;
            startingEventData = JsonUtility.ToJson(StartingEvent);
        }
    }

    // Restore data of the startingEvent
    public void OnAfterDeserialize()
    {
        if (!string.IsNullOrEmpty(startingEventAssemblyQualifiedName) && !string.IsNullOrEmpty(startingEventData))
        {
            // Restore Starting event
            Type eventType = Type.GetType(startingEventAssemblyQualifiedName);
            EventBase eventInstance = (EventBase)ScriptableObject.CreateInstance(eventType);
            JsonUtility.FromJsonOverwrite(startingEventData, eventInstance);
            StartingEvent = eventInstance;
        }
    }
}