using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;
using Unity.VisualScripting;


[Serializable]
[CreateAssetMenu(menuName = "EventInfo/SequentialEventInfo", fileName = "NewSequentialEventInfo")]
public class SequentialEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public List<GameEventInfo> EventInfos => _eventInfos;
    public int StartIndex
    {
        get => _startIndex;
        set => _startIndex = value;
    }

    public void Initialize(List<GameEventInfo> infos, int startInex)
    {
        Assert.IsTrue(infos != null , "GameEventInfo list is null");

        _eventInfos         = infos;
        _startIndex         = startInex;
        IsInitialized       = true;
        IsRuntimeInstance   = true;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.IsRuntimeInstance = true;
        clone._eventInfos = new List<GameEventInfo>();
        foreach(var info in _eventInfos)
        {
            clone._eventInfos.Add(info.Clone());
        }

        return clone;
    }

    public override GameEventDTO ToDTO()
    {
        return new SequentialEventDTO
        {
            EventType   = EventType,
            EventDTOs  = _eventInfos.ConvertAll(info => info.ToDTO()),
            StartIndex  = _startIndex
        };
    }

    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Sequential;
    }

    protected override void OnValidate()
    {
        IsInitialized = EventInfos != null; // && EventDTOs.Count > 0;
    }


    /****** Private Members ******/

    [SerializeField] private List<GameEventInfo> _eventInfos = null;
    [SerializeField] private int _startIndex = 0; // The index of the first event to be played
}