using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using StageEnums;
using CharacterEnums;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager : MonoBehaviour, IAsyncLoadObject
{
    /******* Public Members ********/

    public static DataManager Instance { get; private set; }
    public const int SLOT_NUM = 18;
    public bool IsSaving { get; private set; }
    public bool IsLoaded => _isLoaded;

    public void CreateNewGameData()
    {
        // Initialize Info
        Stage curStage  = Stage.Test;
        int curMap      = 1;
        string saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Get current time
        PlayerType lastChar = PlayerType.Heroine;
        string playTime = "00:00"; 

        // Initialize current data
        _currentData = new UserData(curStage, curMap, null, lastChar, playTime, saveTime);

        // Set player data
        PlayerManager.Instance.SetPlayerData(curStage, curMap, lastChar);
    }

    // Save User Data.
    public void SaveUserData(int slotNum, bool takeScreenShot)
    {
        StartCoroutine(AsyncSaveUserData(slotNum, takeScreenShot));
    }
    private IEnumerator AsyncSaveUserData(int slotNum, bool takeScreenShot)
    {
        // start saving
        IsSaving = true;

        // Update current data according to player data
        PlayerManager.Instance.GetPlayerData(out Stage stage, out int map, out PlayerType character);
        _currentData.UpdatePlayerData(stage, map, character);

        // Wait for a frame before taking a screenshot
        yield return new WaitForEndOfFrame();


        if (takeScreenShot) // Take screenshot of story screen
        {
            Texture2D screenShot = CaptureScreenShot();
            _currentData.SlotImage = screenShot;
            Destroy(screenShot);
        }
        else // Set stage slot image
        {
            _currentData.SlotImage = _stageSlotImage[_currentData.CurrentStage];
        }

        // Calculate play time
        _currentData.PlayTime = CalculatePlayTime(_currentData.PlayTime, _currentData.SaveTime);

        // Update save time
        _currentData.SaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Set starting event
        _currentData.ActiveEventInfoList = GameEventManager.Instance.GetSavableEventInfoList();

        // Save Json file
        string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
        string json = JsonUtility.ToJson(_currentData);
        File.WriteAllText(path, json);

        // Finsih Saving
        IsSaving = false;
    }

    // Capture current screenshot
    private Texture2D CaptureScreenShot()
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

        return screenShot;
    }

    // Calculate play time
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

    // Load specific game slot data and return starting event
    public List<GameEventInfo> LoadGameData(int slotNum)
    {
        string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _currentData = JsonUtility.FromJson<UserData>(json);
        }
        else
        {
            Debug.Log("Trying to read empty slot");
        }

        // Set player data
        PlayerManager.Instance.SetPlayerData(_currentData.CurrentStage, _currentData.CurrentMap, _currentData.LastCharacter);

        return _currentData.ActiveEventInfoList;
    }

    // Load recent saved data and return starting event
    public List<GameEventInfo> LoadRecentData()
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

        // Set player data
        PlayerManager.Instance.SetPlayerData(_currentData.CurrentStage, _currentData.CurrentMap, _currentData.LastCharacter);

        return _currentData.ActiveEventInfoList;
    }

    /****** Private Members ******/

    
    private UserData _currentData;
    private Dictionary<Stage, Texture2D> _stageSlotImage;
    private bool _isLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private IEnumerator Start()
    {
        yield return LoadStageImage();

        // Set loaded flag
        _isLoaded = true;
    }

    // Load stage slot image and save it to dictionary
    private IEnumerator LoadStageImage()
    {
        _stageSlotImage = new Dictionary<Stage, Texture2D>();
        string rootKey = "Stage Slot Image/";
        string test = rootKey + Stage.Test.ToString();

        AsyncOperationHandle<Texture2D> handle = Addressables.LoadAssetAsync<Texture2D>(test);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _stageSlotImage.Add(Stage.Test, handle.Result);
        }
        else
        {
            Debug.LogError("Failed to load addressable asset: " + test);
        }
    }
}