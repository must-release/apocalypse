using UnityEngine;
using EventEnums;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewSceneActivateEvent", menuName = "EventInfo/SceneActivateEvent", order = 0)]
public class SceneActivateEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public bool ShouldTurnOnLoadingUI { get => _shouldTurnOnLoadingUI; private set => _shouldTurnOnLoadingUI = value; }

    public void Initialize(bool shouldTurnOnLoadingUI)
    {
        Assert.IsTrue(false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed.");

        ShouldTurnOnLoadingUI   = shouldTurnOnLoadingUI;
        IsInitialized           = true;
        IsFromAddressables      = false;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.SceneActivate;
    }

    protected override void OnValidate()
    {
        IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private bool _shouldTurnOnLoadingUI = true;
}