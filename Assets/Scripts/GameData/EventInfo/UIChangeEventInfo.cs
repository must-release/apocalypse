using UnityEngine;
using UIEnums;
using EventEnums;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewUIChangeEvent", menuName = "EventInfo/UIChangeEvent", order = 0)]
public class UIChangeEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public BaseUI TargetUI { get { return _targetUI; } private set { _targetUI = value; } }

    public void Initialize(BaseUI targetUI, bool isRuntimeInstance = false)
    {
        Assert.IsTrue(false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed.");

        TargetUI            = targetUI;
        IsInitialized       = true;
        IsRuntimeInstance   = true;
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