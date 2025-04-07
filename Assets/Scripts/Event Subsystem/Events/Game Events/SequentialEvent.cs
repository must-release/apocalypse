using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventEnums;
using UnityEngine.Assertions;


public class SequentialEvent : GameEvent
{
    /****** Public Members ******/

    public void SetEventInfo(SequentialEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "GameEventInfo is not valid");

        _info   = eventInfo;
        Status  = EventStatus.Waiting;

        foreach (GameEventInfo info in _info.EventInfos)
        {
            GameEvent evt = GameEventFactory.CreateFromInfo(info);
            _eventQueue.Enqueue(evt);
        }
    }

    public void AddEvent(GameEvent gameEvent)
    {
        Assert.IsTrue(null != gameEvent, "GameEvent is null");
        Assert.IsTrue(gameEvent.Status == EventStatus.Waiting, "GameEvent is not in waiting state");


        _info.EventInfos.Add(gameEvent.GetEventInfo());
        _eventQueue.Enqueue(gameEvent);
    }

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue( null != _eventQueue, "There are no events to run sequentially" );

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(RunSequentially());
    }

    public override void TerminateEvent()
    {
        if (null != _eventCoroutine)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        ScriptableObject.Destroy(_info);
        _info = null;
        _eventQueue.Clear();

        GameEventPool<SequentialEvent>.Release(this);

        base.TerminateEvent();
    }

    public override GameEventInfo GetEventInfo() => _info;


    /****** Private Members ******/

    private SequentialEventInfo _info       = null;
    private Queue<GameEvent> _eventQueue    = new Queue<GameEvent>();
    private Coroutine _eventCoroutine       = null;

    private IEnumerator RunSequentially()
    {
        Assert.IsTrue(0 < _eventQueue.Count, "There are no events to run sequentially");

        while (0 < _eventQueue.Count)
        {
            GameEvent evt = _eventQueue.Dequeue();
            GameEventManager.Instance.Submit(evt);

            yield return new WaitUntil(() => evt.Status == EventStatus.Terminated);
        }

        TerminateEvent();
    }
}


[CreateAssetMenu(menuName = "EventInfo/SequentialEventInfo", fileName = "NewSequentialEventInfo")]
public class SequentialEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public List<GameEventInfo> EventInfos => _eventInfos;

    public void Initialize(List<GameEventInfo> infos)
    {
        Assert.IsTrue(infos != null , "GameEventInfo list is null");

        _eventInfos = infos;
        IsInitialized = true;
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
}
