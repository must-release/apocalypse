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
    public UserData CreateUserData()
    {
        DateTime now = DateTime.Now;
        string saveTime = now.ToString("yyyy-MM-dd HH:mm:");

        return new UserData(UserData.STAGE.TEST, 0, null, 0, UserData.CHARACTER.HERO, 0, saveTime);
    }

    // Called When PlayerData is modified
    public void AutoSave(UserData data)
    {
        string path = Application.persistentDataPath + "/userData_auto" + ".json";
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    public void SaveUserData(UserData data, int slotNum)
    {
        string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

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




    /******** Manage Story Data **********/

    public StoryEvent CreatePrologueStory()
    {
        StoryEvent prologueStory = ScriptableObject.CreateInstance<StoryEvent>();
        prologueStory.stage = UserData.STAGE.TUTORIAL;
        prologueStory.storyNum = 0;

        return prologueStory;
    }

    // Start loading text of the current story event which player is having 
    public void LoadStoryText()
    {
        StoryEvent storyEvent = (StoryEvent)GameManager.Instance.PlayerData.startingEvent;
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





    /********* Manage Map Data *********/

    // Load two maps when starting the game
    public void LoadMaps()
    {
        UserData data = GameManager.Instance.PlayerData;
        string map1 = "MAP_" + data.currentStage.ToString() + '_' + data.currentMap;
        string map2 = "MAP_" + data.currentStage.ToString() + '_' + (data.currentMap + 1);

        StartCoroutine(AsynLoadMaps(map1, map2));
    }

    public void LoadMap(int stageNum)
    {

    }

    // Load two maps asynchronously
    IEnumerator AsynLoadMaps(string map1, string map2)
    {
        GameObject firstMap = null, secondMap = null;

        // Load first map
        AsyncOperationHandle<GameObject> first = Addressables.LoadAssetAsync<GameObject>(map1);
        yield return first;
        if (first.Status == AsyncOperationStatus.Succeeded)
        {
            firstMap = first.Result;
        }

        // Load second map
        AsyncOperationHandle<GameObject> second = Addressables.LoadAssetAsync<GameObject>(map2);
        yield return second;
        if (second.Status == AsyncOperationStatus.Succeeded)
        {
            secondMap = second.Result;
        }


        // Return two loaded maps to the StageManager
        if (firstMap == null || secondMap == null)
            Debug.Log("Map Load Error");
        else
            StageManager.Instance.OnLoadComplete(firstMap, secondMap);
    }
}

