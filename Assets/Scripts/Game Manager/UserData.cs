using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using StageEnums;
using CharacterEums;


[System.Serializable]
public class UserData : ISerializationCallbackReceiver
{

    [SerializeField] private int currentStage;
    public Stage CurrentStage { get { return (Stage)currentStage; } set { currentStage = (int)value; } }

    [SerializeField] private int currentMap;
    public int CurrentMap { get { return currentMap; } set { currentMap = value; } }

    [SerializeField] private int lastCharacter;
    public PLAYER LastCharacter { get { return (PLAYER)lastCharacter; } set { lastCharacter = (int)value; } }

    [SerializeField] private string playTime;
    public string PlayTime { get { return playTime; } set { playTime = value; } }

    [SerializeField] private string saveTime;
    public string SaveTime { get { return saveTime; } set { saveTime = value; } }

    [SerializeField] private string slotImage;
    public Texture2D SlotImage
    {
        get
        {
            byte[] imageBytes = Convert.FromBase64String(slotImage);

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
            slotImage = Convert.ToBase64String(imageBytes);
        }
    }

    // Used for game
    [NonSerialized] public GameEvent startingEvent;
    public GameEvent StartingEvent { get { return startingEvent; } set { startingEvent = value; } }

    // Used for serialization
    [SerializeField] private string startingEventAssemblyQualifiedName;
    [SerializeField] private string startingEventData;

    public UserData(Stage curStage, int curMap, GameEvent startingEvent, PLAYER lastChar, string playTime, string saveTime)
    {
        CurrentStage = curStage;
        CurrentMap = curMap;
        StartingEvent = startingEvent;
        LastCharacter = lastChar;
        PlayTime = playTime;
        SaveTime = saveTime;
    }

    // Update user data
    public void UpdatePlayerData(Stage stage, int map, PLAYER character)
    {
        CurrentStage = stage;
        CurrentMap = map;
        LastCharacter = character;
    }

    // Save info of the startingEvent
    public void OnBeforeSerialize()
    {
        if (StartingEvent != null)
        {
            // Save chained events
            Stack<GameEvent> eventStack = new Stack<GameEvent>();
            GameEvent savingEvent = StartingEvent;
            while(savingEvent != null)
            {
                eventStack.Push(savingEvent);
                savingEvent = savingEvent.NextEvent;
            }
            while(eventStack.Count > 0)
            {
                eventStack.Pop().SaveNextEventInfo();
            }

            // Create Json data of StartingEvent
            startingEventAssemblyQualifiedName = StartingEvent.GetType().AssemblyQualifiedName;
            startingEventData = JsonUtility.ToJson(StartingEvent);
        }
        else
        {
            startingEventAssemblyQualifiedName = null;
            startingEventData = null;
        }
    }

    // Restore data of the startingEvent
    public void OnAfterDeserialize()
    {
        if (!string.IsNullOrEmpty(startingEventAssemblyQualifiedName) && !string.IsNullOrEmpty(startingEventData))
        {
            // Restore Starting event
            Type eventType = Type.GetType(startingEventAssemblyQualifiedName);
            GameEvent eventInstance = (GameEvent)ScriptableObject.CreateInstance(eventType);
            JsonUtility.FromJsonOverwrite(startingEventData, eventInstance);
            StartingEvent = eventInstance;
            StartingEvent.RestoreNextEventInfo();
        }
    }
}