using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using StageEnums;


[System.Serializable]
public class UserData : ISerializationCallbackReceiver
{
    public enum CHARACTER { HERO, HEROINE }

    [SerializeField]
    private int currentStage;
    public STAGE CurrentStage { get { return (STAGE)currentStage; } set { currentStage = (int)value; } }

    [SerializeField]
    private int currentMap;
    public int CurrentMap { get { return currentMap; } set { currentMap = value; } }

    [SerializeField]
    private int readBlockCount;
    public int ReadBlockCount { get { return readBlockCount; } set { readBlockCount = value; } }

    [SerializeField]
    private int readEntryCount;
    public int ReadEntryCount { get { return readEntryCount; } set { readEntryCount = value; } }

    private int lastCharacter;
    public CHARACTER LastCharacter { get { return (CHARACTER)lastCharacter; } set { lastCharacter = (int)value; } }

    [SerializeField]
    private string playTime;
    public string PlayTime { get { return playTime; } set { playTime = value; } }

    [SerializeField]
    private string saveTime;
    public string SaveTime { get { return saveTime; } set { saveTime = value; } }

    [SerializeField]
    private string screenShotImage;
    public Texture2D ScreenShotImage
    {
        get
        {
            byte[] imageBytes = Convert.FromBase64String(screenShotImage);

            // convert byte array to Texture2D
            Texture2D texture = new Texture2D(2, 2); // Initial size doesn't matter, LoadImage resizes it
            if (texture.LoadImage(imageBytes))
            {
                // Convert Texture2D to Sprite
                return texture;
            }
            else
            {
                return null;
            }
        }
        set
        {
            // Get PNG data
            byte[] imageBytes = value.EncodeToPNG();

            // Convert PNG data to Base64 string
            screenShotImage = Convert.ToBase64String(imageBytes);
        }
    }

    // Used for game
    [System.NonSerialized]
    public EventBase startingEvent;
    public EventBase StartingEvent { get { return startingEvent; } set { startingEvent = value; } }

    // Used for serialization
    [SerializeField]
    private string startingEventAssemblyQualifiedName;
    [SerializeField]
    private string startingEventData;


    public UserData(STAGE curStage, int curMap, EventBase startingEvent,
        int readBlockCnt, int readEntryCnt, CHARACTER lastChar, string playTime, string saveTime)
    {
        CurrentStage = curStage;
        CurrentMap = curMap;
        StartingEvent = startingEvent;
        ReadBlockCount = readBlockCnt;
        ReadEntryCount = readEntryCnt;
        LastCharacter = lastChar;
        PlayTime = playTime;
        SaveTime = saveTime;
    }

    public UserData Copy()
    {
        return new UserData(CurrentStage, CurrentMap, StartingEvent, readBlockCount, readEntryCount, LastCharacter, PlayTime, SaveTime);
    }

    // Save info of the startingEvent
    public void OnBeforeSerialize()
    {
        if (StartingEvent != null)
        {
            // Save chained events
            Stack<EventBase> eventStack = new Stack<EventBase>();
            EventBase nextEvent = StartingEvent.NextEvent;
            while(nextEvent != null)
            {
                eventStack.Push(nextEvent);
                nextEvent = nextEvent.NextEvent;
            }
            while(eventStack.Count > 0)
            {
                eventStack.Pop().SaveNextEventInfo();
            }

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
            StartingEvent.RestoreNextEventInfo();
        }
    }
}