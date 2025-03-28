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
        Assert.IsTrue(eventInfo.IsInitialized, "Event info is not initialized");

        _uiEventInfo = eventInfo;
    }

    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
    {
        if ( null == parentEvent )
            return true;
        
        return false;
    }

    // Play change UI event
    public override void PlayEvent()
    {
        Assert.IsTrue(null != _uiEventInfo, "Event info is not initialized");


        UIController.Instance.ChangeBaseUI(_uiEventInfo.TargetUI); 

        GameEventManager.Instance.TerminateGameEvent(this); 
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _uiEventInfo, "Event info is not set before termination");


        ScriptableObject.Destroy(_uiEventInfo);
        _uiEventInfo = null;

        GameEventPool<UIChangeEvent>.Release(this);
    }

    public override GameEventInfo GetEventInfo()
    {
        return _uiEventInfo;
    }


    /****** Private Members ******/

    private UIChangeEventInfo _uiEventInfo = null;
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