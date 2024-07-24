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
using StageEnums;
using CharacterEums;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public const int SLOT_NUM = 18;
    public bool IsSaving { get; private set; }

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
            new UserData(STAGE.TEST, 0, null, CHARACTER.HERO, "00:00", saveTime);

        // Set current player data
        PlayerManager.Instance.PlayerData = startData;
    }

    // Save User Data.
    public void SaveUserData(int slotNum)
    {
        // start saving
        IsSaving = true;

        // Get current Data
        UserData data = PlayerManager.Instance.PlayerData.Copy();

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

        // Finsih Saving
        IsSaving = false;  
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

    // Load specific game slot data
    public void LoadGameData(int slotNum)
    {
        UserData loadData = null;

        string path = Application.persistentDataPath + "/userData" + slotNum + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            loadData = JsonUtility.FromJson<UserData>(json);
        }
        else
        {
            Debug.Log("Trying to read empty slot");
        }

        // Set current player data
        PlayerManager.Instance.PlayerData = loadData;
    }

    // Load recent saved data
    public void LoadRecentData()
    {
        List<UserData> allData = GetAllUserData();

        // Parse string to DateTime by using DateTime.TryParseExact method
        // Use "yyyy-MM-dd HH:mm" format and CultureInfo.InvariantCulture
        UserData recentData = allData
            .Where(u => u != null && !string.IsNullOrWhiteSpace(u.SaveTime))
            .OrderByDescending(u =>
            {
                DateTime.TryParseExact(u.SaveTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);
                return parsedDate;
            })
            .FirstOrDefault();

        // Set current player data
        PlayerManager.Instance.PlayerData = recentData;
    }
}