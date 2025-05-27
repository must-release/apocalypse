using EventEnums;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/*
 * Load Scene Event
 */

public class SceneLoadEvent : GameEventBase<SceneLoadEventInfo>
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => false;
    public override GameEventInfo   EventInfo       => _info;
    public override GameEventType   EventType       => GameEventType.SceneLoad;

    public override void Initialize(SceneLoadEventInfo eventInfo, IGameEvent parentEvent = null)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info       = eventInfo;
        Status      = EventStatus.Waiting;
        ParentEvent = parentEvent;
    }

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        foreach (GameEventType eventType in activeEventTypeCounts.Keys)
        {
            if (GameEventType.Story == eventType || GameEventType.Choice == eventType || GameEventType.Sequential == eventType)
                continue;

            return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not initialized" );

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not set before termination");

        if (_eventCoroutine != null)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }
        
        _info.DestroyInfo();
        _info = null;

        GameEventPool<SceneLoadEvent, SceneLoadEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private SceneLoadEventInfo _info = null;

    private Coroutine _eventCoroutine = null;

    private IEnumerator PlayEventCoroutine()
    {
        yield return new WaitUntil(() => SceneController.Instance.CanMoveToNextScene);

        SceneController.Instance.LoadGameScene(_info.LoadingScene);

        TerminateEvent();
    }
}