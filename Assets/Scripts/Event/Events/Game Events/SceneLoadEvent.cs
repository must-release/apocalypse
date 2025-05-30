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

    public override bool            ShouldBeSaved       => false;
    public override GameEventType   EventType           => GameEventType.SceneLoad;
    public override bool            IsExclusiveEvent    => true;

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
        Assert.IsTrue(null != Info, "Event info is not initialized" );

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not set before termination");

        if (_eventCoroutine != null)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }
        
        Info.DestroyInfo();
        Info = null;

        GameEventPool<SceneLoadEvent, SceneLoadEventInfo>.Release(this);

        base.TerminateEvent();
    }

    /****** Private Members ******/

    private Coroutine _eventCoroutine = null;

    private IEnumerator PlayEventCoroutine()
    {
        yield return new WaitUntil(() => SceneController.Instance.CanMoveToNextScene);

        SceneController.Instance.LoadGameScene(Info.LoadingScene);

        TerminateEvent();
    }
}