using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreferenceManager : MonoBehaviour
{
    public static PreferenceManager Instance { get; private set; }

    private List<PreferenceObserver> observerList; // Observers which observes setting parameters

    // Settings parameters
    public PreferenceSettings PreferenceSettingInfo { get; private set; }
    public KeySettings KeySettingInfo { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            observerList = new List<PreferenceObserver>();
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
        observerList.ForEach((observer) => observer.PreferenceUpdated());
    }

    public void ChangeKeySettings(KeySettings keySettings)
    {
        KeySettingInfo = keySettings;
        observerList.ForEach((observer) => observer.PreferenceUpdated());
    }

    // Add new observer to the list
    public void AddObserver(PreferenceObserver newObserver)
    {
        observerList.Add(newObserver);
        newObserver.PreferenceUpdated(); // Immediately update the new observer
    }


}

public interface PreferenceObserver
{
    public void PreferenceUpdated();
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
    public KeyCode confirmButton = KeyCode.Return;
}