﻿using UnityEngine;
using UIEnums;
using EventEnums;
using SceneEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSceneLoad", menuName = "Event/SceneLoadEvent", order = 0)]
public class SceneLoadEvent : GameEvent
{
    private int loadingScene;
    public SCENE LoadingScene { get { return (SCENE)loadingScene; }  set { loadingScene = (int)value;  } }

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EventEnums.GameEventType.SceneLoad;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
    {
        // Can be played when parent event is null, story or choice
        if (parentEvent == null || parentEvent.EventType == EventEnums.GameEventType.Story || 
            parentEvent.EventType == EventEnums.GameEventType.Choice)
        {
            return true;
        }
        else
            return false;
    }

    // Play Scene Load Event
    public override void PlayEvent()
    {
        // Load scene asynchronously
        GameSceneController.Instance.LoadGameScene(LoadingScene);

        // Terminate scene load event and play next event
        GameEventManager.Instance.TerminateGameEvent(this);
    }
}

