﻿using UnityEngine;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public EventBase prologueEvent;
    public const int SLOT_NUM = 18;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    /******* Manage User Data ********/

    // Create new game data and set it to current data
    public void CreateNewGameData()
    {
        string saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"); // Get current time

        // Initialize player data
        UserData startData =
            new UserData(UserData.STAGE.TEST, 0, prologueEvent, 0, 0, UserData.CHARACTER.HERO, "00:00", saveTime);

        // Set current player data
        GameManager.Instance.PlayerData = startData;
    }

    // Auto Save data. slot 0 is used for auto save
    public void AutoSaveUserData()
    {
        // When loading auto save data, next event will be played after the loading event
        EventBase loadingEvent = GameEventRouter.Instance.CreateLoadingEvent(GameEventRouter.Instance.NextEvent);
        UserData data = GameManager.Instance.PlayerData.Copy();
        data.StartingEvent = loadingEvent;

        // Capture Screenshot and save data
        StartCoroutine(CaptureScreenshotAndSave(data, 0));
    }

    // Save User Data.
    public void SaveUserData(int slotNum)
    {
        // When loading save data, current event will be played after the loading event
        EventBase loadingEvent = GameEventRouter.Instance.CreateLoadingEvent(GameEventRouter.Instance.CurrentEvent);
        UserData data = GameManager.Instance.PlayerData.Copy();
        data.StartingEvent = loadingEvent;

        // Set story dialogue number, which shows last text again
        data.ReadBlockCount = StoryModel.Instance.ReadBlockCount;
        data.ReadEntryCount = StoryModel.Instance.ReadEntryCount;

        // Capture Screenshot and save data
        StartCoroutine(CaptureScreenshotAndSave(data, slotNum));
    }

    // Capture a screenshot of current game
    private IEnumerator CaptureScreenshotAndSave(UserData data, int slotNum)
    {
        // Wait for a frame to end before taking a screenshot
        yield return new WaitForEndOfFrame();

        /******* Capture Screenshot *********/
        {
            // Create RenderTexture which has equal a equal with screen
            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            // Set current rendered content to RenderTexture
            RenderTexture.active = renderTexture;
            Camera.main.targetTexture = renderTexture;
            Camera.main.Render();

            // Copy pixel info from RenderTexture to Texture2D
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenShot.Apply();

            // Reset RenderTexture and Camera settings
            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture);

            // Set screenshot image
            data.ScreenShotImage = screenShot;
            Destroy(screenShot);
        }


        /******* Save User Data *********/
        {
            // Calculate play time
            data.PlayTime = CalculatePlayTime(data.PlayTime, data.SaveTime);

            // Update save time
            data.SaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            // Save Json file
            string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(path, json);
        }

        // if it's not autosave, refresh UI
        if(slotNum != 0)
        {
            UIController.Instance.RefreshState();
        }
    }

    // Calculate play time
    private string CalculatePlayTime(string existingPlayTime, string saveTime)
    {
        DateTime currentTime = DateTime.Now;

        // Parse save time
        DateTime saveDateTime;
        if (!DateTime.TryParseExact(saveTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out saveDateTime))
        {
            Console.WriteLine("Save time format is incorrect.");
            return existingPlayTime;
        }

        // Parse existing play time. Parse Hours and Minutes seperately
        string[] parts = existingPlayTime.Split(':');
        if (parts.Length != 2 || !int.TryParse(parts[0], out int existingHours) || !int.TryParse(parts[1], out int existingMinutes))
        {
            Console.WriteLine("Existing play time format is incorrect.");
            return existingPlayTime;
        }
        TimeSpan existingTimeSpan = new TimeSpan(existingHours, existingMinutes, 0);

        // Get current play time
        TimeSpan timePlayedThisSession = currentTime - saveDateTime;
        TimeSpan updatedPlayTime = existingTimeSpan + timePlayedThisSession;

        int totalHours = (int)updatedPlayTime.TotalHours;
        int minutes = updatedPlayTime.Minutes;

        return $"{totalHours:00}:{minutes:00}";
    }

    // Load every user data
    public List<UserData> GetAllUserData()
    {
        List<UserData> allData = new List<UserData>();

        for(int slotNum = 0; slotNum < SLOT_NUM; slotNum++)
        {
            string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                UserData data = JsonUtility.FromJson<UserData>(json);
                allData.Add(data);
            }
            else
            {
                allData.Add(null);
            }
        }

        return allData;
    }

    // Load most recent saved data and start loading stage
    public void LoadContinueData()
    {
        List<UserData> allData = GetAllUserData();

        // Parse string to DateTime by using DateTime.TryParseExact method
        // Use "yyyy-MM-dd HH:mm" format and CultureInfo.InvariantCulture
        UserData mostRecent = allData
            .Where(u => u != null && !string.IsNullOrWhiteSpace(u.SaveTime))
            .OrderByDescending(u =>
            {
                DateTime.TryParseExact(u.SaveTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);
                return parsedDate;
            })
            .FirstOrDefault();

        // Set current player data
        GameManager.Instance.PlayerData = mostRecent;
    }



    /******** Manage Story Data **********/

    // Start loading text of the current story event which player is having 
    public void LoadStoryText()
    {
        string storyInfo = GameEventRouter.Instance.CurrentEvent.GetEventInfo<string>();
        Addressables.LoadAssetAsync<TextAsset>(storyInfo).Completed += OnStoryLoadComplete;
    }
    private void OnStoryLoadComplete(AsyncOperationHandle<TextAsset> story)
    {
        // Set JsonConvert settings
        var settings = new JsonSerializerSettings
        {
            // Add custom converter
            Converters = new List<JsonConverter> { new StoryEntryConverter() }
        };

        // Convert Json file to StoryEntries object
        string jsonContent = story.Result.text;
        StoryBlocks storyBlocks = JsonConvert.DeserializeObject<StoryBlocks>(jsonContent, settings);

        // Set story info
        StoryModel.Instance.SetStoryInfo(storyBlocks.blocks);
    }

    // Class for converting story json file
    class StoryEntryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(StoryEntry));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            switch (jo["type"].Value<string>())
            {
                case "dialogue":
                    return jo.ToObject<Dialogue>(serializer);
                case "effect":
                    return jo.ToObject<Effect>(serializer);
                case "choice":
                    return jo.ToObject<Choice>(serializer);
                default:
                    throw new Exception("Unknown type");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }





    /********* Manage Stage Assets *********/

    // Load two maps when starting the game
    public void LoadMaps()
    {
        UserData data = GameManager.Instance.PlayerData;
        string map1 = "MAP_" + data.CurrentStage.ToString() + '_' + data.CurrentMap;
        string map2 = "MAP_" + data.CurrentStage.ToString() + '_' + (data.CurrentMap + 1);

        StartCoroutine(AsyncLoadMaps(map1, map2));
    }

    // Load a map while playing the game
    public void LoadMap(int stageNum)
    {

    }

    // Load two maps asynchronously
    IEnumerator AsyncLoadMaps(string map1, string map2)
    {
        GameObject firstMap = null, secondMap = null;

        // Load first map
        AsyncOperationHandle<GameObject> first = Addressables.InstantiateAsync(map1);
        yield return first;
        if (first.Status == AsyncOperationStatus.Succeeded)
        {
            firstMap = first.Result;
        }

        // Load second map
        AsyncOperationHandle<GameObject> second = Addressables.InstantiateAsync(map2);
        yield return second;
        if (second.Status == AsyncOperationStatus.Succeeded)
        {
            secondMap = second.Result;
        }


        // Return two loaded maps to the GameSceneManager
        if (firstMap == null || secondMap == null)
            Debug.Log("Map Load Error");
        else
            SceneController.Instance.OnMapsLoadComplete(firstMap, secondMap);
    }

    // Start to load player prefab
    public void LoadPlayer()
    {

        string key = "Characters/Player";
        Addressables.InstantiateAsync(key).Completed += AsyncLoadPlayer;
    }

    // When player is loaded, return player instance to GameSceneManager
    private void AsyncLoadPlayer(AsyncOperationHandle<GameObject> playerInstance)
    {
        GameObject player = null;

        if(playerInstance.Status == AsyncOperationStatus.Succeeded)
        {
            player = playerInstance.Result;
        }

        if (player == null)
            Debug.Log("Player Load Error");
        else
            SceneController.Instance.onPlayerLoadComplete(player);
    }
}