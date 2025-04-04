using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventEnums;
using UnityEngine.Assertions;


public class SequentialEvent : GameEvent
{
    /****** Public Members ******/

    public void SetEventInfo(SequentialEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not initialized");

        _info = eventInfo;
        Status = EventStatus.Waiting;
    }

    public override bool CheckCompatibility()
    {
        return true;
    }

    public override void PlayEvent()
    {
        base.PlayEvent();
        StartCoroutine(RunSequentially());
    }

    private IEnumerator RunSequentially()
    {
        foreach (GameEventInfo info in _info.EventInfos)
        {
            GameEvent evt = GameEventFactory.CreateFromInfo(info);
            GameEventManager.Instance.Submit(evt);
            yield return new WaitUntil(() => evt.Status == EventStatus.Terminated);
        }

        TerminateEvent();
    }

    protected override void TerminateEvent()
    {
        base.TerminateEvent();
    }

    public override GameEventInfo GetEventInfo() => _info;


    /****** Private Members ******/

    private SequentialEventInfo _info;
}


[CreateAssetMenu(menuName = "EventInfo/SequentialEventInfo", fileName = "NewSequentialEventInfo")]
public class SequentialEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public List<GameEventInfo> EventInfos => _eventInfos;

    public void Initialize(List<GameEventInfo> infos)
    {
        Assert.IsTrue(infos != null && infos.Count > 0, "GameEventInfo list is not set");

        _eventInfos = infos;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Sequential;
    }

    protected override void OnValidate()
    {
        IsInitialized = EventInfos != null && EventInfos.Count > 0;
    }


    /****** Private Members ******/

    [SerializeField] private List<GameEventInfo> _eventInfos;
}
