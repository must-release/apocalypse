using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventEnums;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;


public class SequentialEvent : GameEventBase<SequentialEventInfo>, IEventContainer
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => true;
    public override GameEventType   EventType       => GameEventType.Sequential;


    public override void Initialize(SequentialEventInfo eventInfo)
    {
        base.Initialize(eventInfo);

        _currentEventIndex = Info.StartIndex;

        for (int i = _currentEventIndex; i < Info.EventInfos.Count; ++i)
        {
            GameEventInfo info = Info.EventInfos[i];
            IGameEvent evt = GameEventFactory.CreateFromInfo(info);
            _eventQueue.Enqueue(evt);
            evt.EventContainer = this;
        }
    }

    public void AddNewEvent(IGameEvent newEvent)
    {
        Assert.IsTrue(null != newEvent, "GameEvent is null");
        Assert.IsTrue(newEvent.Status == EventStatus.Waiting, "GameEvent is not in waiting state");

        Info.EventInfos.Add(newEvent.EventInfo);
        _eventQueue.Enqueue(newEvent);
        newEvent.EventContainer = this;
    }

    public bool IsContainingEvent(IGameEvent findingEvent)
    {
        return _eventQueue.Contains(findingEvent);
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

        Info.DestroyInfo();
        Info = null;
        _eventQueue.Clear();

        GameEventPool<SequentialEvent, SequentialEventInfo>.Release(this);

        base.TerminateEvent();
    }

    public override void UpdateStatus()
    {
        base.UpdateStatus();

        Info.StartIndex = _currentEventIndex;
        _eventQueue.Peek().UpdateStatus();
    }


    /****** Private Members ******/

    private Queue<IGameEvent>   _eventQueue = new Queue<IGameEvent>();
        
    private Coroutine   _eventCoroutine     = null;
    private int         _currentEventIndex  = 0;

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
