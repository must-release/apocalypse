using UnityEngine;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewSceneLoadEventInfo", menuName = "EventInfo/SceneLoadEvent", order = 0)]
public class SceneLoadEventInfo : GameEventInfo, ISerializableEventInfo
{
    /****** Public Members ******/

    public SceneType LoadingScene { get { return _loadingScene; } private set { _loadingScene = value; }}

    public void Initialize(SceneType loadingScene)
    {
        Debug.Assert( false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed." );

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

    public GameEventDTO ToDTO()
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
        if ( SceneType.SceneTypeCount != LoadingScene )
            IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private SceneType _loadingScene = SceneType.SceneTypeCount;
}