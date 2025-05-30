using UnityEngine;
using SceneEnums;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewSceneLoadEventInfo", menuName = "EventInfo/SceneLoadEvent", order = 0)]
public class SceneLoadEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public SceneName LoadingScene { get { return _loadingScene; } private set { _loadingScene = value; }}

    public void Initialize(SceneName loadingScene, bool isRuntimeInstance = false)
    {
        Assert.IsTrue( false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed." );

        LoadingScene        = loadingScene;
        IsInitialized       = true;
        IsRuntimeInstance   = true;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.IsRuntimeInstance = true;

        return clone;
    }

    public override GameEventDTO ToDTO()
    {
        return new SceneLoadEventDTO
        {
            EventType = EventType,
            LoadingScene = _loadingScene
        };
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.SceneLoad;
    }

    protected override void OnValidate()
    {
        if ( SceneName.SceneNameCount != LoadingScene )
            IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private SceneName _loadingScene = SceneName.SceneNameCount;
}