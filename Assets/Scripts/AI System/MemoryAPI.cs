using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MemoryAPI : MonoBehaviour
{
	public static MemoryAPI Instance;

    public static string UserID = "user4350";
    public static string saveURL = "http://ec2-54-153-172-184.ap-southeast-2.compute.amazonaws.com:8080/memory/save";
    public static string initURL = "http://ec2-54-153-172-184.ap-southeast-2.compute.amazonaws.com:8080/memory/init";
    public static string responseURL = "http://ec2-54-153-172-184.ap-southeast-2.compute.amazonaws.com:8080/response/generate";
    public static string reflectURL = "http://ec2-54-153-172-184.ap-southeast-2.compute.amazonaws.com:8080/memory/reflect";
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Load inital Story
        //Addressables.LoadAssetAsync<TextAsset>("INITIAL_STORY_KOREAN").Completed += OnInitStoryLoadComplete;
        Addressables.LoadAssetAsync<TextAsset>("INITIAL_STORY_AMERICAN").Completed += OnInitStoryLoadComplete;
    }
    private void OnInitStoryLoadComplete(AsyncOperationHandle<TextAsset> story)
    {
        // Convert the object to JSON format
        string jsonData = story.Result.text;
        StartCoroutine(InitMemoryCoroutine(jsonData));
    }
    IEnumerator InitMemoryCoroutine(string jsonData)
    {

        // Create a UnityWebRequest for sending POST requests
        UnityWebRequest webRequest = new UnityWebRequest(initURL, "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            UnityEngine.Debug.Log("Error: " + webRequest.error);
        }
        else
        {
            UnityEngine.Debug.Log("Received: " + webRequest.downloadHandler.text);

            UnityEngine.Debug.Log("Init compelete");
        }
    }






    public void SaveMemory(Dialogue dialogue)
    {
        StartCoroutine(SaveMemoryCoroutine(dialogue));
    }
    IEnumerator SaveMemoryCoroutine(Dialogue dialogue)
    {
        // Set Memory attributes
        SaveMemory memory = new SaveMemory();
        memory.userId = UserID;
        memory.isDescription = false;
        if(dialogue.character.Equals("나"))
            memory.content = "지성이 " + dialogue.text + "라고 말했다.";
        else if(dialogue.character.Equals("연아"))
            memory.content = "나는 " + dialogue.text + "라고 말했다.";
        else
        {
            memory.content = dialogue.text;
            memory.isDescription = true;
        }
        memory.playTime = "00:00";
        memory.importance = 0.5f;

        // Convert the object to JSON format
        string jsonData = JsonUtility.ToJson(memory);

        // Create a UnityWebRequest for sending POST requests
        UnityWebRequest webRequest = new UnityWebRequest(saveURL, "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            UnityEngine.Debug.Log("Error: " + webRequest.error);
        }
        else
        {
            UnityEngine.Debug.Log("Received: " + webRequest.downloadHandler.text);
            UnityEngine.Debug.Log("save compelete");
        }
    }



    public void ResponseMemory(Dialogue dialogue,int responseCount)
    {
        StartCoroutine(ResponseMemoryCoroutine(dialogue, responseCount));
    }

    IEnumerator ResponseMemoryCoroutine(Dialogue dialogue, int responseCount)
    {
        ResponseMemory memory = new ResponseMemory();
        memory.userId = UserID;
        memory.content = "지성은 " + dialogue.text + "라고 말했다.";
        memory.playTime = "00:00";
        memory.importance = 0.5f;
        memory.count = responseCount;


        // Convert the object to JSON format
        string jsonData = JsonUtility.ToJson(memory);

        // Create a UnityWebRequest for sending POST requests
        UnityWebRequest webRequest = new UnityWebRequest(responseURL, "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            UnityEngine.Debug.Log("Error: " + webRequest.error);
            StoryController.Instance.ShowResponse("오류", false);
        }
        else
        {
            UnityEngine.Debug.Log("Received: " + webRequest.downloadHandler.text);
            // Deserialize the JSON response
            ResponseData response = JsonUtility.FromJson<ResponseData>(webRequest.downloadHandler.text);

            StoryController.Instance.ShowResponse(response.content, response.regenerate);
        }
    }

    public void Reflect()
    {
        StartCoroutine(ReflectCoroutine());
    }
    IEnumerator ReflectCoroutine()
    {
        Reflect reflect = new Reflect();
        reflect.userId = UserID;

        // Convert the object to JSON format
        string jsonData = JsonUtility.ToJson(reflect);

        // Create a UnityWebRequest for sending POST requests
        UnityWebRequest webRequest = new UnityWebRequest(reflectURL, "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            UnityEngine.Debug.Log("Error: " + webRequest.error);
        }
        else
        {
            UnityEngine.Debug.Log("Received: " + webRequest.downloadHandler.text);
            UnityEngine.Debug.Log("Reflect compelete");
        }
    }
}




[System.Serializable]
class SaveMemory
{
    public string userId;
    public string content;
    public string playTime;
    public float importance;
    public bool isDescription;
}

[System.Serializable]
class ResponseMemory
{
    public string userId;
    public string content;
    public string playTime;
    public float importance;
    public int count;
}

[System.Serializable]
public class ResponseData
{
    public string content;
    public bool regenerate;
}

[System.Serializable]
public class Reflect
{
    public string userId;
}