using UnityEngine;
using System.Collections;
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
    public override GameEventType   EventType       => GameEventType.Cutscene;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Debug.Assert(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        if (activeEventTypeCounts.Count == 0)
            return true;
        
        return false;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not set.");

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");


        if ( _eventCoroutine != null )
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        Info.DestroyInfo();
        Info = null;

        GameEventPool<CutsceneEvent, CutsceneEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private Coroutine       _eventCoroutine     = null;

    private IEnumerator PlayEventCoroutine()
    {
        Debug.Assert(null != Info, "Event info is not set.");

        // Change to cutscene UI
        UIController.Instance.ChangeBaseUI(BaseUI.Cutscene);

        // Play cutscene
        GamePlayManager.Instance.PlayCutscene();

        // Wait for cutscene to end
        yield return new WaitUntil( () => GamePlayManager.Instance.IsCutscenePlaying );

        TerminateEvent();
    }
}