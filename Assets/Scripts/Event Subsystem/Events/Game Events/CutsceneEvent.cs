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

public class CutsceneEvent : GameEvent
{
    /****** Public Members ******/

    public override bool ShouldBeSaved() => false;

    public void SetEventInfo(CutsceneEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info   = eventInfo;
        Status  = EventStatus.Waiting;
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

        // Todo: Release the event info
        // if (_info.IsFromAddressables) Addressables.Release(_info);
        // else Destroy(_info);
        _info = null;

        GameEventPool<CutsceneEvent>.Release(this);

        base.TerminateEvent();
    }

    public override GameEventInfo GetEventInfo() => _info;

    public override GameEventType GetEventType() => GameEventType.Cutscene;


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