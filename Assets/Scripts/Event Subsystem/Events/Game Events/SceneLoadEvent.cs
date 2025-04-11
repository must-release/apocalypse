using UnityEngine;
using EventEnums;
using SceneEnums;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

/*
 * Load Scene Event
 */

public class SceneLoadEvent : GameEvent
{
    /****** Public Members ******/

    public override bool ShouldBeSaved() => false;

    public void SetEventInfo(SceneLoadEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info   = eventInfo;
        Status  = EventStatus.Waiting;
    }

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        foreach (GameEventType eventType in activeEventTypeCounts.Keys)
        {
            if (GameEventType.Story == eventType || GameEventType.Choice == eventType || GameEventType.Sequential == eventType)
                continue;

            return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not initialized" );

        base.PlayEvent();

        // Load scene asynchronously
        GameSceneController.Instance.LoadGameScene(_info.LoadingScene);

        // Terminate scene load event and play next event
        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not set before termination");

        ScriptableObject.Destroy(_info);
        _info = null;

        GameEventPool<SceneLoadEvent>.Release(this);

        base.TerminateEvent();
    }

    public override GameEventInfo GetEventInfo() => _info;

    public override GameEventType GetEventType() => GameEventType.SceneLoad;


    /****** Private Members ******/

    private SceneLoadEventInfo _info = null;
}