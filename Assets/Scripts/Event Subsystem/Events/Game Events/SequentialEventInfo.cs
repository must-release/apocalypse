using UnityEngine;
using System.Collections.Generic;
using EventEnums;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(menuName = "EventInfo/SequentialEventInfo", fileName = "NewSequentialEventInfo")]
public class SequentialEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public List<GameEventInfo> EventInfos => _eventInfos;
    public int StartIndex => _startIndex;

    public void Initialize(List<GameEventInfo> infos)
    {
        Assert.IsTrue(infos != null , "GameEventInfo list is null");

        _eventInfos         = infos;
        IsInitialized       = true;
        IsFromAddressables  = false;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Sequential;
    }

    protected override void OnValidate()
    {
        IsInitialized = EventInfos != null; // && EventInfos.Count > 0;
    }


    /****** Private Members ******/

    [SerializeField] private List<GameEventInfo> _eventInfos = null;
    [SerializeField] private int _startIndex = 0; // The index of the first event to be played
}