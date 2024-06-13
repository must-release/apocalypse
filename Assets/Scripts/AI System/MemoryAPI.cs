using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MemoryAPI : MonoBehaviour
{
	public static MemoryAPI Instance;

    public static string UserID;
    public static string baseURL = "http://sw.uos.ac.kr:8080/";
    public static string saveURL = baseURL+ "memory/save";
    public static string initURL = baseURL + "memory/init";
    public static string responseURL = baseURL + "response/generate";
    public static string reflectURL = baseURL + "memory/reflect";
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Generate a unique UserID for testing
        UserID = "User" + System.DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        // Load inital Story
        Addressables.LoadAssetAsync<TextAsset>("INITIAL_STORY_KOREAN").Completed += OnInitStoryLoadComplete;
        //Addressables.LoadAssetAsync<TextAsset>("INITIAL_STORY_AMERICAN").Completed += OnInitStoryLoadComplete;
    }
    private void OnInitStoryLoadComplete(AsyncOperationHandle<TextAsset> story)
    {
        // Convert the object to JSON format
        string jsonData = story.Result.text;

        // Parse JSON data
        JObject jsonObject = JObject.Parse(jsonData);

        // Update the userId in the JSON object
        jsonObject["userId"] = UserID;

        // Convert the JSON object back to a string
        jsonData = jsonObject.ToString();

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

        if(responseCount == StoryController.MAX_RESPONSE_COUNT)
        {
            memory.instruction = "Instruct : 너는 지금 지성과 대화 중이고, 지성이 말한 마지막 대사와 대화 맥락을 고려하여 자연스럽게 대화를 마무리해야 한다." +
            "<이전 대화내용>에는 지금까지 지성과 나눈 대화와 상황이 순서대로 기록되어 있다." +
            "답변을 할 때 <이전 대화내용> 에 있는 지성의 마지막 대사를 반복하지 않는다." +
            "답변을 할 때 <연아의 기억>을 반영한다. index가 높을 수록 최근 기억이고, priority가 높을 수록 우선적으로 참고해야할 기억이다. " +
            "생성된 답변은 <연아의 기억>에 있는 기억들과 일관성을 가져야 한다." +
            "'나는 ~라고 말했다'같은 특정한 양식 없이, 평범하게 사람처럼 답변을 말해야 한다." +
            "여러 문장으로 답변할 경우 각 문장을 개행한다.";
        }


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
            StoryController.Instance.ShowResponse("오류");
        }
        else
        {
            UnityEngine.Debug.Log("Received: " + webRequest.downloadHandler.text);
            // Deserialize the JSON response
            ResponseData response = JsonUtility.FromJson<ResponseData>(webRequest.downloadHandler.text);

            StoryController.Instance.ShowResponse(response.content);
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
    public string instruction = "Instruct : 너는 지금 지성과 대화 중이고, 지성이 말한 마지막 대사와 대화 맥락을 고려하여 적절한 답변을 말해야 한다." +
        "<이전 대화내용>에는 지금까지 지성과 나눈 대화와 상황이 순서대로 기록되어 있다." +
        "답변을 할 때 <이전 대화내용>에 있는 지성의 마지막 대사를 반복하지 않는다." +
        "답변을 할 때 <연아의 기억>을 반영한다. index가 높을 수록 최근 기억이고, priority가 높을 수록 우선적으로 참고해야할 기억이다. " +
        "답변은 <연아의 기억>에 있는 기억들과 일관성을 가져야 한다." +
        "'나는 ~라고 말했다'같은 특정한 양식 없이, 평범하게 사람처럼 답변을 말해야 한다." +
        "여러 문장으로 답변할 경우 각 문장을 개행한다.";
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