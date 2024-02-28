using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager: MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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

    public void LoadStoryText(int stageNum, int storyNum)
    {
        string story = "Story" + stageNum + '-' + storyNum;
        Addressables.LoadAssetAsync<TextAsset>(story).Completed += OnStoryLoadComplete;
    }

    public void OnStoryLoadComplete(AsyncOperationHandle<TextAsset> story)
    {
        string jsonContent = story.Result.text;
        Queue<DialogueEntry> scriptEntries = JsonUtility.FromJson<Queue<DialogueEntry>>(jsonContent);
    }

    public void LoadMap(int stageNum, int mapNum, int nextMapNum)
    {
        string key1 = "Temp/Map" + stageNum + '-' + mapNum;
        string key2 = "Temp/Map" + stageNum + '-' + nextMapNum;

        StartCoroutine(LoadTwoMaps(key1, key2));
    }

    IEnumerator LoadTwoMaps(string key1, string key2)
    {
        GameObject firstMap = null, secondMap = null;

        // Load first map
        AsyncOperationHandle<GameObject> first = Addressables.LoadAssetAsync<GameObject>(key1);
        yield return first;
        if (first.Status == AsyncOperationStatus.Succeeded)
        {
            firstMap = first.Result;
        }

        // Load second map
        AsyncOperationHandle<GameObject> second = Addressables.LoadAssetAsync<GameObject>(key2);
        yield return second;
        if (second.Status == AsyncOperationStatus.Succeeded)
        {
            secondMap = second.Result;
        }


        if (firstMap == null || secondMap == null)
            Debug.Log("Map Load Error");
        else
            StageManager.Instance.OnLoadComplete(firstMap, secondMap);
    }
}

