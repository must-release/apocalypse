using UnityEngine;
using UIEnums;
using EventEnums;
using SceneEnums;
using UnityEngine.Assertions;

/*
 * Load Scene Event
 */

public class SceneLoadEvent : GameEvent
{
    /****** Public Members ******/

    public void SetEventInfo(SceneLoadEventInfo eventInfo)
    {
        Assert.IsTrue(eventInfo.IsInitialized, "Event info is not initialized");

        _sceneLoadEventInfo = eventInfo;
    }

    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
    {
        if (parentEvent == null || parentEvent.EventType == GameEventType.Story || parentEvent.EventType == GameEventType.Choice)
            return true;
        
        return false;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != _sceneLoadEventInfo, "Event info is not initialized" );

        // Load scene asynchronously
        GameSceneController.Instance.LoadGameScene(_sceneLoadEventInfo.LoadingScene);

        // Terminate scene load event and play next event
        GameEventManager.Instance.TerminateGameEvent(this);
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _sceneLoadEventInfo, "Event info is not set before termination");

        ScriptableObject.Destroy(_sceneLoadEventInfo);
        _sceneLoadEventInfo = null;

        GameEventPool<SceneLoadEvent>.Release(this);
    }

    public override GameEventInfo GetEventInfo()
    {
        return _sceneLoadEventInfo;
    }


    /****** Private Members ******/

    private SceneLoadEventInfo _sceneLoadEventInfo = null;
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