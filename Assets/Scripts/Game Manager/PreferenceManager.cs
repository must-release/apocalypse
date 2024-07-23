using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreferenceManager : MonoBehaviour
{
    public static PreferenceManager Instance { get; private set; }

    // Observers which observes setting parameters
    private List<PreferenceObserver> preferenceObservers;
    private List<KeySettingObserver> keySettingObservers;


    // Settings parameters
    public PreferenceSettings PreferenceSettingInfo { get; private set; }
    public KeySettings KeySettingInfo { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            preferenceObservers = new List<PreferenceObserver>();
            keySettingObservers = new List<KeySettingObserver>();
            PreferenceSettingInfo = new PreferenceSettings();
            KeySettingInfo = new KeySettings();
        }
    }

    private void Start()
    {
        
    }

    public void ChangePreference(PreferenceSettings preference)
    {
        PreferenceSettingInfo = preference;
        preferenceObservers.ForEach((observer) => observer.PreferenceUpdated());
    }

    public void ChangeKeySettings(KeySettings keySettings)
    {
        KeySettingInfo = keySettings;
        keySettingObservers.ForEach((observer) => observer.KeySettingUpdated());
    }

    // Add new observer to the list
    public void AddObserver(PreferenceObserver newObserver)
    {
        preferenceObservers.Add(newObserver);
        newObserver.PreferenceUpdated(); // Immediately update the new observer
    }
    public void AddObserver(KeySettingObserver newObserver)
    {
        keySettingObservers.Add(newObserver);
        newObserver.KeySettingUpdated(); // Immediately update the new observer
    }

}

public interface PreferenceObserver
{
    public void PreferenceUpdated();
}

public interface KeySettingObserver
{
    public void KeySettingUpdated();
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