using UnityEngine;
using EventEnums;
using SceneEnums;
using UnityEngine.Assertions;
using System.Collections.Generic;

/*
 * Load Scene Event
 */

public class SceneLoadEvent : GameEvent
{
    /****** Public Members ******/

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
            if (GameEventType.Story == eventType || GameEventType.Choice == eventType)
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

    public override GameEventInfo GetEventInfo()
    {
        return _info;
    }


    /****** Private Members ******/

    private SceneLoadEventInfo _info = null;
}


[CreateAssetMenu(fileName = "NewSceneLoadEvent", menuName = "EventInfo/SceneLoadEvent", order = 0)]
public class SceneLoadEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public Scene LoadingScene { get { return _loadingScene; } private set { _loadingScene = value; }}

    public void Initialize(Scene loadingScene)
    {
        Assert.IsTrue( false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed." );

        LoadingScene    = loadingScene;
        IsInitialized   = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.SceneLoad;
    }

    protected override void OnValidate()
    {
        if ( Scene.SceneCount != LoadingScene )
            IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private Scene _loadingScene = Scene.SceneCount;
}