using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEnums;
using EventEnums;
using UnityEngine.Assertions;

public class UIChangeEvent : GameEvent
{
    /****** Public Members ******/

    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
    {
        if ( null == parentEvent )
            return true;
        
        return false;
    }

    // Play change UI event
    public override void PlayEvent(GameEventInfo eventInfo)
    {
        Assert.IsTrue(GameEventType.UIChange == eventInfo.EventType, "잘못된 이벤트 타입[" + eventInfo.EventType.ToString() + "]입니다. UIChangeEventInfo를 전달해주세요.");

        _uiEventInfo = eventInfo as UIChangeEventInfo;
        UIController.Instance.ChangeBaseUI(_uiEventInfo.TargetUI); 
        GameEventManager.Instance.TerminateGameEvent(this); 
    }

    public override GameEventInfo GetEventInfo()
    {
        return _uiEventInfo;
    }

    /****** Private Members ******/

    private UIChangeEventInfo _uiEventInfo;
}


[CreateAssetMenu(fileName = "NewUIChange", menuName = "EventInfo/UIChange", order = 0)]
public class UIChangeEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public BaseUI TargetUI { get { return _targetUI; } private set { _targetUI = value; } }

    public void Initialize(BaseUI targetUI)
    {
        Assert.IsTrue(false == IsInitialized, "GameEventInfo의 중복 초기화는 허용하지 않습니다.");

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

    private BaseUI _targetUI = BaseUI.BaseUICount;
}