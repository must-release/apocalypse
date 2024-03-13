using UnityEngine;
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
using UnityEditor;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public EventBase prologueEvent;

    private const int SLOT_NUM = 18;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    /******* Manage User Data ********/

    // Create new game data
    public void CreateUserData()
    {
        string saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:"); // Get current time

        // Initialize player data
        GameManager.Instance.PlayerData =
            new UserData(UserData.STAGE.TEST, 0, prologueEvent, 0, UserData.CHARACTER.HERO, "00:00", saveTime);
    }

    // Auto Save data. slot 0 is used for auto save
    public void AutoSaveUserData()
    {
        // When loading auto save data, next event will be played after the loading event
        LoadingEvent loading = ScriptableObject.CreateInstance<LoadingEvent>();
        loading.Initialize(EventManager.Instance.NextEvent);
        UserData data = GameManager.Instance.PlayerData.Copy();
        data.StartingEvent = loading;

        // Calculate play time
        data.PlayTime = CalculatePlayTime(data.PlayTime, data.SaveTime);
        
        // Save Json file
        string path = Application.persistentDataPath + "/userData0.json";
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    // Save User Data.
    public void SaveUserData(int slotNum)
    {
        UserData data = null;
        string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
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
    public List<UserData> LoadAllUserData()
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

    // Load most recent saved Data
    public void LoadContinueData()
    {
        List<UserData> allData = LoadAllUserData();

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

        GameManager.Instance.PlayerData = mostRecent;
    }




    /******** Manage Story Data **********/

    // Start loading text of the current story event which player is having 
    public void LoadStoryText()
    {
        StoryEvent storyEvent = (StoryEvent)EventManager.Instance.CurrentEvent;
        string story = "STORY_" + storyEvent.stage.ToString() + '_' + storyEvent.storyNum;
        Addressables.LoadAssetAsync<TextAsset>(story).Completed += OnStoryLoadComplete;
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
        StoryEntries scriptEntries = JsonConvert.DeserializeObject<StoryEntries>(jsonContent, settings);

        // Set story which starts at the recent point to the StorUI
        IStoryInfo storyInfo = StoryUIState.Instance;
        int lastDlg = GameManager.Instance.PlayerData.LastDialogueNumber;
        storyInfo.StoryQueue = new Queue<StoryEntry>(scriptEntries.entries.Skip(lastDlg));
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
            JObject item = JObject.Load(reader);
            switch (item["type"].Value<string>())
            {
                case "dialogue":
                    return item.ToObject<Dialogue>(serializer);
                case "effect":
                    return item.ToObject<Effect>(serializer);
                case "choice":
                    return item.ToObject<Choice>(serializer);
                default:
                    throw new NotImplementedException($"Unrecognized type: {item["type"].Value<string>()}");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Serialize back if necessary. You might not need this if you only deserialize.
            throw new NotImplementedException();
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


        // Return two loaded maps to the StageManager
        if (firstMap == null || secondMap == null)
            Debug.Log("Map Load Error");
        else
            StageManager.Instance.OnMapsLoadComplete(firstMap, secondMap);
    }

    // Start to load player prefab
    public void LoadPlayer()
    {
        string key = "Characters/Player";
        Addressables.InstantiateAsync(key).Completed += AsyncLoadPlayer;
    }

    // When player is loaded, return player instance to StageManager
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
            StageManager.Instance.onPlayerLoadComplete(player);
    }
}

