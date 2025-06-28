using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

public class DataManager : MonoBehaviour, IAsyncLoadObject
{
    /******* Public Members ********/

    public static DataManager Instance { get; private set; }
    public static int SlotNumber => 18;

    public bool IsSaving    { get; private set; }
    public bool IsLoaded    => _isLoaded;  

    public void CreateNewGameData()
    {
        PlayerType  lastChar    = PlayerType.Hero;
        ChapterType curChapter  = ChapterType.Test;
        int         curStage    = 1;
        string      saveTime    = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string      playTime    = "00:00"; 

        _currentData = new UserData(curChapter, curStage, null, lastChar, playTime, saveTime);
        PlayerManager.Instance.SetPlayerData(curChapter, curStage, lastChar);
    }

    public void SaveUserData(List<GameEventDTO> dtoList, int slotNum, bool takeScreenShot)
    {
        StartCoroutine(AsyncSaveUserData(dtoList, slotNum, takeScreenShot));
    }

    public List<UserData> GetAllUserData()
    {
        List<UserData> allData = new List<UserData>();

        for(int slotNum = 0; slotNum < SlotNumber; slotNum++)
        {
            string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                UserData data = JsonConvert.DeserializeObject<UserData>(json, _jsonSettings);
                allData.Add(data);
            }
            else
            {
                allData.Add(null);
            }
        }

        return allData;
    }

    public List<GameEventDTO> LoadGameData(int slotNum)
    {
        string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _currentData = JsonConvert.DeserializeObject<UserData>(json, _jsonSettings);
        }
        else
        {
            Debug.Log("Trying to read empty slot");
        }

        PlayerManager.Instance.SetPlayerData(_currentData.CurrentStage, _currentData.CurrentMap, _currentData.LastCharacter);

        return _currentData.ActiveEventDTOList;
    }

    public List<GameEventDTO> LoadRecentData()
    {
        List<UserData> allData = GetAllUserData();

        // Parse string to DateTime by using DateTime.TryParseExact method
        // Use "yyyy-MM-dd HH:mm" format and CultureInfo.InvariantCulture
        _currentData = allData
            .Where(u => u != null && !string.IsNullOrWhiteSpace(u.SaveTime))
            .OrderByDescending(u =>
            {
                DateTime.TryParseExact(u.SaveTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);
                return parsedDate;
            })
            .FirstOrDefault();

        PlayerManager.Instance.SetPlayerData(_currentData.CurrentStage, _currentData.CurrentMap, _currentData.LastCharacter);

        return _currentData.ActiveEventDTOList;
    }


    /****** Private Members ******/

    private Dictionary<ChapterType, Texture2D>    _stageSlotImage    = new();
    private JsonSerializerSettings          _jsonSettings      = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

    private UserData    _currentData;
    private bool        _isLoaded;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private IEnumerator Start()
    {
        yield return LoadStageImage();

        _isLoaded = true;
    }

    private IEnumerator LoadStageImage()
    {
        string rootKey  = "Stage Slot Image/";
        string test     = rootKey + ChapterType.Test.ToString();

        AsyncOperationHandle<Texture2D> handle = Addressables.LoadAssetAsync<Texture2D>(test);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _stageSlotImage.Add(ChapterType.Test, handle.Result);
        }
        else
        {
            Debug.LogError("Failed to load addressable asset: " + test);
        }
    }

    private IEnumerator AsyncSaveUserData(List<GameEventDTO> eventList, int slotNum, bool takeScreenShot)
    {
        IsSaving = true;

        PlayerManager.Instance.GetPlayerData(out ChapterType stage, out int map, out PlayerType character);
        _currentData.UpdatePlayerData(stage, map, character);

        // Wait for a frame before taking a screenshot
        yield return new WaitForEndOfFrame();

        if (takeScreenShot)
        {
            Texture2D screenShot = CaptureScreenShot();
            _currentData.SlotImage = screenShot;
            Destroy(screenShot);
        }
        else
        {
            _currentData.SlotImage = _stageSlotImage[_currentData.CurrentStage];
        }


        _currentData.PlayTime = CalculatePlayTime(_currentData.PlayTime, _currentData.SaveTime);
        _currentData.SaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _currentData.ActiveEventDTOList = eventList;

        string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
        string json =JsonConvert.SerializeObject(_currentData, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
        File.WriteAllText(path, json);

        IsSaving = false;
    }

    private Texture2D CaptureScreenShot()
    {
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        RenderTexture.active = renderTexture;
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        return screenShot;
    }

    private string CalculatePlayTime(string existingPlayTime, string saveTime)
    {
        DateTime currentTime = DateTime.Now;

        // Parse save time
        DateTime saveDateTime;
        if (!DateTime.TryParseExact(saveTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out saveDateTime))
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
}