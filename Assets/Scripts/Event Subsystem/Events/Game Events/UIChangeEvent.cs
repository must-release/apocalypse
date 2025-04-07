using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEnums;
using EventEnums;
using UnityEngine.Assertions;

public class UIChangeEvent : GameEvent
{
    /****** Public Members ******/

    public void SetEventInfo(UIChangeEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info   = eventInfo;
        Status  = EventStatus.Waiting;
    }

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        if (0 == activeEventTypeCounts.Count)
            return true;
        
        return false;
    }

    // Play change UI event
    public override void PlayEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not initialized");

        base.PlayEvent();
        UIController.Instance.ChangeBaseUI(_info.TargetUI); 

        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not set before termination");


        ScriptableObject.Destroy(_info);
        _info = null;

        GameEventPool<UIChangeEvent>.Release(this);

        base.TerminateEvent();
    }

    public override GameEventInfo GetEventInfo() => _info;


    /****** Private Members ******/

    private UIChangeEventInfo _info = null;
}


[CreateAssetMenu(fileName = "NewUIChangeEvent", menuName = "EventInfo/UIChangeEvent", order = 0)]
public class UIChangeEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public BaseUI TargetUI { get { return _targetUI; } private set { _targetUI = value; } }

    public void Initialize(BaseUI targetUI)
    {
        Assert.IsTrue( false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed." );

        TargetUI        = targetUI;
        IsInitialized   = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.UIChange;
    }

    protected override void OnValidate()
    {
        if ( BaseUI.BaseUICount != TargetUI)
            IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private BaseUI _targetUI = BaseUI.BaseUICount;
}