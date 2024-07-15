using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private List<SettingsObserver> observerList; // Observers which observes setting parameters

    // Settings parameters
    private bool isFullScreen;
    private Vector2 resolution;
    private float bgmVolume;
    private float soundEffectVolume;
    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            observerList = new List<SettingsObserver>();
        }
    }

    // Add new observer to the list
    public void AddObserver(SettingsObserver newObserver)
    {
        observerList.Add(newObserver);
    }


}

public interface SettingsObserver
{
    public void SettingsUpdated();
}