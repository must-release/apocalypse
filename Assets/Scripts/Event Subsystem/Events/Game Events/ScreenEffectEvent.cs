using ScreenEffectEnums;
using EventEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;

/*
 * Screen effect event. Must be removed.
 */

public class ScreenEffectEvent : GameEventBase<ScreenEffectEventInfo>
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => false;
    public override GameEventInfo   EventInfo       => _info;
    public override GameEventType   EventType       => GameEventType.ScreenEffect;

    public override void Initialize(ScreenEffectEventInfo eventInfo, IGameEvent parentEvent = null)
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
            if (GameEventType.Story == eventType || GameEventType.Cutscene == eventType)
                continue;

            return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not initialized");

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

        GameEventPool<ScreenEffectEvent, ScreenEffectEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/
    private Coroutine               _eventCoroutine     = null;
    private ScreenEffectEventInfo   _info               = null;

    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue( null != _info, "Event info should be set" );


        ScreenEffecter screenEffecter = UtilityManager.Instance.GetUtilityTool<ScreenEffecter>();

        switch (_info.ScreenEffectType)
        {
            case ScreenEffect.FadeIn:
                _eventCoroutine = screenEffecter.FadeIn();
                break;
            case ScreenEffect.FadeOut:
                _eventCoroutine = screenEffecter.FadeOut(); 
                break;
        }

        yield return _eventCoroutine;

        TerminateEvent();
    }
}