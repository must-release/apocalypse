using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreferenceManager : MonoBehaviour
{
    public static PreferenceManager Instance { get; private set; }

    private List<PreferenceObserver> observerList; // Observers which observes setting parameters

    // Settings parameters
    public bool isFullScreen;
    public Vector2 resolution;
    public float bgmVolume;
    public float soundEffectVolume;
    public KeySettings keySettings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            observerList = new List<PreferenceObserver>();
            keySettings = new KeySettings();
        }
    }

    private void Start()
    {
        
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