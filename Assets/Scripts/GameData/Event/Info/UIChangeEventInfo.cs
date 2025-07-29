using UnityEngine;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewUIChangeEventInfo", menuName = "EventInfo/UIChangeEvent", order = 0)]
public class UIChangeEventInfo : GameEventInfo, ISerializableEventInfo
{
    /****** Public Members ******/

    public BaseUI TargetUI { get { return _targetUI; } private set { _targetUI = value; } }

    public void Initialize(BaseUI targetUI, bool isRuntimeInstance = false)
    {
        Debug.Assert(false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed.");

        TargetUI            = targetUI;
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
        return new UIChangeEventDTO
        {
            EventType = EventType,
            TargetUI = _targetUI
        };
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