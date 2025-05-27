using UnityEngine;
using EventEnums;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewDataSaveEvent", menuName = "EventInfo/DataSaveEvent", order = 0)]
public class DataSaveEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public int  SlotNum { get { return _slotNum; } private set { _slotNum = value; }}

    public void Initialize(int slotNum, bool isRuntimeInstnace = false)
    {
        Assert.IsTrue( false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed." );

        SlotNum             = slotNum;
        IsInitialized       = true;
        IsRuntimeInstance   = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.DataSave;
    }

    protected override void OnValidate()
    {
        if ( 0 < _slotNum )
            IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private int _slotNum = -1; // if 0 auto save, else save in slot
}