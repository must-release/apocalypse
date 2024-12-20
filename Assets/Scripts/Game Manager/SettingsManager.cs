﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private List<PreferenceSettingsObserver> preferenceSettingsObservers;
    private List<KeySettingsObserver> keySettingsObservers;

    public PreferenceSettings PreferenceSettingInfo { get; private set; }
    public KeySettings KeySettingInfo { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;            
            preferenceSettingsObservers = new List<PreferenceSettingsObserver>();
            keySettingsObservers = new List<KeySettingsObserver>();

            InitializeSettings();
            LoadSettings();
            PrintCurrentSettings();

            KeySettingInfo.cancelButton = new GameButton("Cancel", KeyCode.Escape);
            KeySettingInfo.confirmButton = new GameButton("Confirm", KeyCode.Return);
            KeySettingInfo.pauseButton = new GameButton("Pause", KeyCode.Escape);

            
            ChangeKeySettings(KeySettingInfo);
            PrintCurrentSettings();

        }
    }

    private void SaveSettings() // Saving both PreferenceSettingInfo and KeySettingInfo as json file
    {
        try
        {
            string preferenceSettingsJson = JsonUtility.ToJson(PreferenceSettingInfo);
            string keySettingsJson = JsonUtility.ToJson(KeySettingInfo, true);

            string preferenceSettingsPath = Application.persistentDataPath + "/PreferenceSettingInfo.json";
            string keySettingsPath = Application.persistentDataPath + "/KeySettingInfo.json";

            File.WriteAllText(preferenceSettingsPath, preferenceSettingsJson);
            File.WriteAllText(keySettingsPath, keySettingsJson);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving settings: " + e.Message);
        }
    }

    private void InitializeSettings() // If there are no saved files , enter default values
    {
        string preferenceSettingsPath = Application.persistentDataPath + "/PreferenceSettingInfo.json";
        string keySettingsPath = Application.persistentDataPath + "/KeySettingInfo.json";

        if (!File.Exists(preferenceSettingsPath) || !File.Exists(keySettingsPath))
        {
            PreferenceSettingInfo = new PreferenceSettings
            {
                isFullScreen = true,
                resolutionWidth = 1920,
                resolutionHeight = 1080,
                bgmVolume = 100,
                soundEffectVolume = 100
            };

            KeySettingInfo = new KeySettings
            {
                cancelButton = new GameButton("Cancel", KeyCode.Escape),
                pauseButton = new GameButton("Pause", KeyCode.Escape),
                confirmButton = new GameButton("Confirm", KeyCode.Return),
                upButton = new GameButton("Up", KeyCode.W),
                rightButton = new GameButton("Right", KeyCode.D),
                leftButton = new GameButton("Left", KeyCode.A),
                downButton = new GameButton("Down", KeyCode.S),
                attackButton = new GameButton("Attack", KeyCode.Mouse0),
                aimButton = new GameButton("Aim", KeyCode.Mouse1),
                specialAttackButton = new GameButton("Special Attack", KeyCode.Mouse2),
                tagButton = new GameButton("Tag", KeyCode.Tab),
                assistAttackButton = new GameButton("Assist Attack", KeyCode.Q),
                interactionButton = new GameButton("Interaction", KeyCode.E),
                jumpButton = new GameButton("Jump", KeyCode.Space),
            };

            SaveSettings();
        }
    }

    private void LoadSettings() // if there are saved files, load them and enter values into PreferenceSettingInfo, KeySettingInfo
    {
        string preferenceSettingsPath = Application.persistentDataPath + "/PreferenceSettingInfo.json";
        string keySettingsPath = Application.persistentDataPath + "/KeySettingInfo.json";

        if (File.Exists(preferenceSettingsPath))
        {

            string preferenceSettingsJson = File.ReadAllText(preferenceSettingsPath);
            PreferenceSettingInfo = JsonUtility.FromJson<PreferenceSettings>(preferenceSettingsJson);
            //Debug.Log("Preference Settings Loaded from file.");
        }

        if (PreferenceSettingInfo == null)
        {
            Debug.LogError("PreferenceSettingInfo is null after loading.");
            PreferenceSettingInfo = new PreferenceSettings();
        }

        if (File.Exists(keySettingsPath))
        {

            string keySettingsJson = File.ReadAllText(keySettingsPath);
            KeySettingInfo = JsonUtility.FromJson<KeySettings>(keySettingsJson);
            //Debug.Log("Key Settings Loaded from file.");
        }

        if (KeySettingInfo == null)
        {
            Debug.LogError("KeySettingInfo is null after loading.");
            KeySettingInfo = new KeySettings();
        }
    }

    public void LoadPreferenceSettings() // Used when only load Preference
    {
        try
        {
            string preferenceSettingsPath = Application.persistentDataPath + "/PreferenceSettingInfo.json";

            if (File.Exists(preferenceSettingsPath))
            {
                string preferenceSettingsJson = File.ReadAllText(preferenceSettingsPath);
                PreferenceSettingInfo = JsonUtility.FromJson<PreferenceSettings>(preferenceSettingsJson);
                //Debug.Log("Preference Settings Loaded: " + preferenceSettingsJson);
            }
            else
            {
                Debug.LogWarning("Preference settings file not found.");
                PreferenceSettingInfo = new PreferenceSettings();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading preference settings: " + e.Message);
        }
    }

    public void LoadKeySettings() // Used when only load Key Settings
    {
        try
        {
            string keySettingsPath = Application.persistentDataPath + "/KeySettingInfo.json";

            if (File.Exists(keySettingsPath))
            {
                string keySettingsJson = File.ReadAllText(keySettingsPath);
                KeySettingInfo = JsonUtility.FromJson<KeySettings>(keySettingsJson);
                //Debug.Log("Key Settings Loaded: " + keySettingsJson);
            }
            else
            {
                Debug.LogWarning("Key settings file not found.");
                KeySettingInfo = new KeySettings();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading key settings: " + e.Message);
        }
    }

    private void Start()
    {
        
    }

    public void ChangePreference(PreferenceSettings newPreferenceInfo)
    {
        PreferenceSettingInfo = newPreferenceInfo;
        SaveSettings();
        preferenceSettingsObservers.ForEach((observer) => observer.PreferenceSettingsUpdated());
        //Debug.Log("Preference settings have been updated.");
    }

    public void ChangeKeySettings(KeySettings newKeySettingInfo)
    {
        KeySettingInfo = newKeySettingInfo;
        SaveSettings();
        keySettingsObservers.ForEach((observer) => observer.KeySettingsUpdated());
        PrintCurrentSettings();
        //Debug.Log("Key settings have been updated.");
    }

    // Add new observer to the list
    public void AddObserver(PreferenceSettingsObserver newObserver)
    {
        preferenceSettingsObservers.Add(newObserver);
        newObserver.PreferenceSettingsUpdated(); // Immediately update the new observer
    }
    public void AddObserver(KeySettingsObserver newObserver)
    {
        keySettingsObservers.Add(newObserver);
        newObserver.KeySettingsUpdated(); // Immediately update the new observer
    }

    public void PrintCurrentSettings()
    {

        // string preferenceSettingsPath = Application.persistentDataPath + "/PreferenceSettingInfo.json";
        // string keySettingsPath = Application.persistentDataPath + "/KeySettingInfo.json";

        // if (File.Exists(preferenceSettingsPath))
        // {
        //     try
        //     {
        //         string preferenceSettingsJson = File.ReadAllText(preferenceSettingsPath);
        //         PreferenceSettings preferenceSettings = JsonUtility.FromJson<PreferenceSettings>(preferenceSettingsJson);
        //         Debug.Log("Current Preference Settings:");
        //         Debug.Log($"FullScreen: {preferenceSettings.isFullScreen}");
        //         Debug.Log($"Resolution: {preferenceSettings.resolutionWidth} x {preferenceSettings.resolutionHeight}");
        //         Debug.Log($"BGM Volume: {preferenceSettings.bgmVolume}");
        //         Debug.Log($"Sound Effect Volume: {preferenceSettings.soundEffectVolume}");
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError("Error reading Preference Settings: " + e.Message);
        //     }
        // }
        // else
        // {
        //     Debug.LogWarning("Preference settings file not found.");
        // }

        // if (File.Exists(keySettingsPath))
        // {
        //     try
        //     {
        //         string keySettingsJson = File.ReadAllText(keySettingsPath);
        //         Debug.Log($"Read Key Settings JSON: {keySettingsJson}"); // JSON 문자열 출력
        //         KeySettings keySettings = JsonUtility.FromJson<KeySettings>(keySettingsJson);
        //         Debug.Log("Current Key Settings:");
        //         Debug.Log($"Cancel Button: {keySettings.cancelButton}");
        //         Debug.Log($"Pause Button: {keySettings.pauseButton}");
        //         Debug.Log($"Confirm Button: {keySettings.confirmButton}");
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError("Error reading Key Settings: " + e.Message);
        //     }
        // }
        // else
        // {
        //     Debug.LogWarning("Key settings file not found.");
        // }
    }
}

public interface PreferenceSettingsObserver
{
     void PreferenceSettingsUpdated();
}

public interface KeySettingsObserver
{
    public void KeySettingsUpdated();
}

[System.Serializable]
public class PreferenceSettings
{
    public bool isFullScreen;
    public float resolutionWidth;
    public float resolutionHeight;
    public float bgmVolume;
    public float soundEffectVolume;
}

[System.Serializable]
public class GameButton
{
    public string buttonName;
    public KeyCode buttonKeyCode;

    public GameButton(string buttonName, KeyCode buttonKeyCode)
    {
        this.buttonName = buttonName;
        this.buttonKeyCode = buttonKeyCode;
    }
}

[System.Serializable]
public class KeySettings
{
    public GameButton cancelButton;
    public GameButton pauseButton;
    public GameButton confirmButton;
    public GameButton upButton;
    public GameButton rightButton;
    public GameButton leftButton;
    public GameButton downButton;
    public GameButton jumpButton;
    public GameButton attackButton;
    public GameButton assistAttackButton;
    public GameButton aimButton;
    public GameButton specialAttackButton;
    public GameButton tagButton;
    public GameButton interactionButton;
}