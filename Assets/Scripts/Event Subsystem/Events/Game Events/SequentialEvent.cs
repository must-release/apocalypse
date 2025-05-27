using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventEnums;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;


public class SequentialEvent : GameEventBase<SequentialEventInfo>
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => true;
    public override GameEventInfo   EventInfo       => _info;
    public override GameEventType   EventType       => GameEventType.Sequential;


    public override void Initialize(SequentialEventInfo eventInfo, IGameEvent parentEvent = null)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "GameEventInfo is not valid");

        _info       = eventInfo;
        Status      = EventStatus.Waiting;
        ParentEvent = parentEvent;  

        _currentEventIndex = _info.StartIndex;

        for (int i = _currentEventIndex; i < _info.EventInfos.Count; ++i)
        {
            GameEventInfo info = _info.EventInfos[i];
            IGameEvent evt = GameEventFactory.CreateFromInfo(info, this);
            _eventQueue.Enqueue(evt);
        }
    }

    public void AddEvent(IGameEvent gameEvent)
    {
        Assert.IsTrue(null != gameEvent, "GameEvent is null");
        Assert.IsTrue(gameEvent.Status == EventStatus.Waiting, "GameEvent is not in waiting state");


        _info.EventInfos.Add(gameEvent.EventInfo);
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

        _info.DestroyInfo();
        _info = null;
        _eventQueue.Clear();

        GameEventPool<SequentialEvent, SequentialEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private SequentialEventInfo _info       = null;
    private Queue<IGameEvent> _eventQueue    = new Queue<IGameEvent>();
    private Coroutine _eventCoroutine       = null;

    private int _currentEventIndex = 0;

    private IEnumerator RunSequentially()
    {
        Assert.IsTrue(0 < _eventQueue.Count, "There are no events to run sequentially");

        while (0 < _eventQueue.Count)
        {
            IGameEvent evt = _eventQueue.Peek();
            GameEventManager.Instance.Submit(evt);

            yield return new WaitUntil(() => evt.Status == EventStatus.Terminated);

            _eventQueue.Dequeue();
            _currentEventIndex++;
        }

        TerminateEvent();
    }
}
