using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using StageEnums;
using CharacterEums;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public const int SLOT_NUM = 18;
    public bool IsSaving { get; private set; }

    private UserData currentData;
    private Dictionary<Stage, Texture2D> stageSlotmage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Load stage slot image
            LoadStageImage();
        }
    }

    // Load stage slot image and save it to dictionary
    private void LoadStageImage()
    {
        stageSlotmage = new Dictionary<Stage, Texture2D>();
        string rootKey = "Stage Slot Image/";
        string test = rootKey + Stage.Test.ToString();

        Addressables.LoadAssetAsync<Texture2D>(test).Completed += (AsyncOperationHandle<Texture2D> img) =>
        {
            if (img.Status == AsyncOperationStatus.Succeeded)
            {
                stageSlotmage.Add(Stage.Test, img.Result);
            }
            else
            {
                Debug.LogError("Failed to load addressable asset: " + test);
            }
        };
    }

    /******* Manage User Data ********/

    // Create new game data and set it to current data
    public void CreateNewGameData()
    {
        // Initialize Info
        string saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Get current time
        Stage curStage = Stage.Test;
        int curMap = 3;
        GameEvent startingEvent = null;
        PLAYER lastChar = PLAYER.HEROINE;
        string playTime = "00:00";

        // Initialize current data
        currentData = new UserData(curStage, curMap, startingEvent, lastChar, playTime, saveTime);

        // Set player data
        PlayerManager.Instance.SetPlayerData(curStage, curMap, lastChar);
    }

    // Save User Data.
    public void SaveUserData(GameEvent startingEvent, int slotNum, bool takeScreenShot)
    {
        StartCoroutine(AsyncSaveUserData(startingEvent, slotNum, takeScreenShot));
    }
    private IEnumerator AsyncSaveUserData(GameEvent startingEvent, int slotNum, bool takeScreenShot)
    {
        // start saving
        IsSaving = true;

        // Update current data according to player data
        PlayerManager.Instance.GetPlayerData(out Stage stage, out int map, out PLAYER character);
        currentData.UpdatePlayerData(stage, map, character);

        // Wait for a frame before taking a screenshot
        yield return new WaitForEndOfFrame();


        if (takeScreenShot) // Take screenshot of story screen
        {
            Texture2D screenShot = CaptureScreenShot();
            currentData.SlotImage = screenShot;
            Destroy(screenShot);
        }
        else // Set stage slot image
        {
            currentData.SlotImage = stageSlotmage[currentData.CurrentStage];
        }

        // Calculate play time
        currentData.PlayTime = CalculatePlayTime(currentData.PlayTime, currentData.SaveTime);

        // Update save time
        currentData.SaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Set starting event
        currentData.StartingEvent = startingEvent;

        // Save Json file
        string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
        string json = JsonUtility.ToJson(currentData);
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
    public GameEvent LoadGameData(int slotNum)
    {
        string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentData = JsonUtility.FromJson<UserData>(json);
        }
        else
        {
            Debug.Log("Trying to read empty slot");
        }

        // Set player data
        PlayerManager.Instance.SetPlayerData(currentData.CurrentStage, currentData.CurrentMap, currentData.LastCharacter);

        return currentData.StartingEvent;
    }

    // Load recent saved data and return starting event
    public GameEvent LoadRecentData()
    {
        List<UserData> allData = GetAllUserData();

        // Parse string to DateTime by using DateTime.TryParseExact method
        // Use "yyyy-MM-dd HH:mm" format and CultureInfo.InvariantCulture
        currentData = allData
            .Where(u => u != null && !string.IsNullOrWhiteSpace(u.SaveTime))
            .OrderByDescending(u =>
            {
                DateTime.TryParseExact(u.SaveTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);
                return parsedDate;
            })
            .FirstOrDefault();

        // Set player data
        PlayerManager.Instance.SetPlayerData(currentData.CurrentStage, currentData.CurrentMap, currentData.LastCharacter);

        return currentData.StartingEvent;
    }
}