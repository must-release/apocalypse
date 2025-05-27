using UnityEngine;
using System.Collections;
using UIEnums;
using EventEnums;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

/*
 * 게임 플레이 도중 컷씬을 재생하는 CutsceneEvent를 관리하는 파일입니다.
 */

public class CutsceneEvent : GameEventBase<CutsceneEventInfo>
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => false;
    public override GameEventInfo   EventInfo       => _info;
    public override GameEventType   EventType       => GameEventType.Cutscene;

    public override void Initialize(CutsceneEventInfo eventInfo, IGameEvent parentEvent = null)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info       = eventInfo;
        Status      = EventStatus.Waiting;
        ParentEvent = parentEvent;
    }   

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        if (activeEventTypeCounts.Count == 0)
            return true;
        
        return false;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not set.");

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not set before termination");


        if ( _eventCoroutine != null )
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        _info.DestroyInfo();
        _info = null;

        GameEventPool<CutsceneEvent, CutsceneEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private Coroutine       _eventCoroutine     = null;
    private GameEventInfo   _info  = null;

    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue(null != _info, "Event info is not set.");

        // Change to cutscene UI
        UIController.Instance.ChangeBaseUI(BaseUI.Cutscene);

        // Play cutscene
        GamePlayManager.Instance.PlayCutscene();

        // Wait for cutscene to end
        yield return new WaitUntil( () => GamePlayManager.Instance.IsCutscenePlaying );

        TerminateEvent();
    }
}