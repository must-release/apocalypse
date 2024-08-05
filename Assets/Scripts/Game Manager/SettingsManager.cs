using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // Observers which observes setting parameters
    private List<PreferenceSettingsObserver> preferenceSettingsObservers;
    private List<KeySettingsObserver> keySettingsObservers;


    // Settings parameters
    public PreferenceSettings PreferenceSettingsInfo { get; private set; }
    public KeySettings KeySettingsInfo { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            preferenceSettingsObservers = new List<PreferenceSettingsObserver>();
            keySettingsObservers = new List<KeySettingsObserver>();
            PreferenceSettingsInfo = new PreferenceSettings();
            KeySettingsInfo = new KeySettings();
        }
    }

    private void Start()
    {
        
    }

    public void ChangePreference(PreferenceSettings preference)
    {
        PreferenceSettingsInfo = preference;
        preferenceSettingsObservers.ForEach((observer) => observer.PreferenceSettingsUpdated());
    }

    public void ChangeKeySettings(KeySettings keySettings)
    {
        KeySettingsInfo = keySettings;
        keySettingsObservers.ForEach((observer) => observer.KeySettingsUpdated());
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

}

public interface PreferenceSettingsObserver
{
    public void PreferenceSettingsUpdated();
}

public interface KeySettingsObserver
{
    public void KeySettingsUpdated();
}

public class PreferenceSettings
{
    public bool isFullScreen = true;
    public Vector2 resolution;
    public float bgmVolume;
    public float soundEffectVolume;
}

public class KeySettings
{
    public KeyCode cancelButton = KeyCode.Escape;
    public KeyCode pauseButton = KeyCode.Escape;
    public KeyCode nextScriptButton = KeyCode.Return;
}