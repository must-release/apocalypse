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
    public override GameEventType   EventType       => GameEventType.ScreenEffect;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Debug.Assert(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

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
        Debug.Assert(null != Info, "Event info is not initialized");

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");

        if (_eventCoroutine != null)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        Info.DestroyInfo();
        Info = null;

        GameEventPool<ScreenEffectEvent, ScreenEffectEventInfo>.Release(this);

        base.TerminateEvent();
    }

    /****** Private Members ******/

    private Coroutine _eventCoroutine = null;

    private IEnumerator PlayEventCoroutine()
    {
        Debug.Assert( null != Info, "Event info should be set" );


        ScreenEffecter screenEffecter = UtilityManager.Instance.GetUtilityTool<ScreenEffecter>();

        switch (Info.ScreenEffectType)
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