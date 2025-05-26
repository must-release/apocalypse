using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventEnums;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;


public class SequentialEvent : GameEvent
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => true;
    public override GameEventInfo   EventInfo       => _info;
    public override GameEventType   EventType       => GameEventType.Sequential;


    public void SetEventInfo(SequentialEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "GameEventInfo is not valid");

        _info   = eventInfo;
        Status  = EventStatus.Waiting;

        _currentEventIndex = _info.StartIndex;

        for (int i = _currentEventIndex; i < _info.EventInfos.Count; ++i)
        {
            GameEventInfo info = _info.EventInfos[i];
            GameEvent evt = GameEventFactory.CreateFromInfo(info);
            _eventQueue.Enqueue(evt);
        }
    }

    public void AddEvent(GameEvent gameEvent)
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

        // Todo: Release the event info
        // if (_info.IsFromAddressables) Addressables.Release(_info);
        // else Destroy(_info);
        _info = null;
        _eventQueue.Clear();

        GameEventPool<SequentialEvent>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private SequentialEventInfo _info       = null;
    private Queue<GameEvent> _eventQueue    = new Queue<GameEvent>();
    private Coroutine _eventCoroutine       = null;

    private int _currentEventIndex = 0;

    private IEnumerator RunSequentially()
    {
        Assert.IsTrue(0 < _eventQueue.Count, "There are no events to run sequentially");

        while (0 < _eventQueue.Count)
        {
            GameEvent evt = _eventQueue.Dequeue();
            GameEventManager.Instance.Submit(evt);

            yield return new WaitUntil(() => evt.Status == EventStatus.Terminated);

            _currentEventIndex++;
        }

        TerminateEvent();
    }
}
