using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreferenceManager : MonoBehaviour
{
    public static PreferenceManager Instance { get; private set; }

    private List<PreferenceObserver> observerList; // Observers which observes setting parameters

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
            observerList = new List<PreferenceObserver>();
        }
    }

    private void Start()
    {
        // Initialize every preference observers
        observerList.ForEach((observer) => observer.PreferenceUpdated());
    }

    // Add new observer to the list
    public void AddObserver(PreferenceObserver newObserver)
    {
        observerList.Add(newObserver);
    }
}

public interface PreferenceObserver
{
    public void PreferenceUpdated();
}