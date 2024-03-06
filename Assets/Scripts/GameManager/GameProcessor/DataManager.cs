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

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

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
        IEvent prologueStory = CreatePrologueStory(); // Create event chain

        // Initialize player data
        GameManager.Instance.PlayerData =
            new UserData(UserData.STAGE.TEST, 0, prologueStory, 0, UserData.CHARACTER.HERO, "00:00", saveTime);
    }

    // Auto Save data. slot 0 is used for auto save
    public void AutoSave()
    {
        // When loading auto save data, next event will be played after the loading event
        LoadingEvent loading = ScriptableObject.CreateInstance<LoadingEvent>();
        loading.Initialize(EventManager.Instance.CurrentEvent.NextEvent);
        UserData data = GameManager.Instance.PlayerData.Copy();
        data.StartingEvent = loading;

        // Calculate play time
        

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

    public string CalculatePlayTime(DateTime now, DateTime past)
    {
        return null;
    }

    // Load user data from a specific data slot
    public UserData LoadUserData(int slotNum)
    {
        string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            UserData data = JsonUtility.FromJson<UserData>(json);
            return data;
        }
        return null;
    }

    // Load most recent saved Data
    public void LoadContinueData()
    {
        string path = Application.persistentDataPath + "/userData0.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            UserData data = JsonUtility.FromJson<UserData>(json);
            GameManager.Instance.PlayerData = data;
        }
        else
            Debug.Log("reading continue data failed");
    }




    /******** Manage Story Data **********/

    // Create prologue story event which have 2 events behind
    public StoryEvent CreatePrologueStory()
    {
        // Create in-game event
        InGameEvent prologueInGame = ScriptableObject.CreateInstance<InGameEvent>();
        prologueInGame.Initialize(null);

        // Create auto save event
        AutoSaveEvent saveEvent = ScriptableObject.CreateInstance<AutoSaveEvent>();
        saveEvent.Initialize(prologueInGame);

        // Create loading event
        LoadingEvent loadingEvent = ScriptableObject.CreateInstance<LoadingEvent>();
        loadingEvent.Initialize(saveEvent);

        // Create story event
        StoryEvent prologueStory = ScriptableObject.CreateInstance<StoryEvent>();
        prologueStory.Initialize(UserData.STAGE.TUTORIAL, 0, loadingEvent);

        return prologueStory;
    }

    // Start loading text of the current story event which player is having 
    public void LoadStoryText()
    {
        StoryEvent storyEvent = (StoryEvent)EventManager.Instance.CurrentEvent;
        string story = "STORY_" + storyEvent.stage.ToString() + '_' + storyEvent.storyNum;
        Addressables.LoadAssetAsync<TextAsset>(story).Completed += OnStoryLoadComplete;
    }

    public void OnStoryLoadComplete(AsyncOperationHandle<TextAsset> story)
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
        int lastDlg = GameManager.Instance.PlayerData.lastDialogueNum;
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
        string map1 = "MAP_" + data.currentStage.ToString() + '_' + data.currentMap;
        string map2 = "MAP_" + data.currentStage.ToString() + '_' + (data.currentMap + 1);

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
    public void AsyncLoadPlayer(AsyncOperationHandle<GameObject> playerInstance)
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

