﻿using UnityEngine;
using System.Collections.Generic;
using UIEnums;
using EventEnums;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;

/*
 * 스토리 진행 중 화면에 선택지를 표시하고, 플레이어가 고른 선택지를 처리하는 ChoiceEvent입니다.
 */

public class ChoiceEvent : GameEvent
{
    /****** Public Members ******/

    public void SetEventInfo(ChoiceEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info       = eventInfo;
        Status      = EventStatus.Waiting;
    }

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        if (activeEventTypeCounts.ContainsKey(GameEventType.Story))
            return true;
        
        return false;
    }

    public override bool ShouldBeSaved() => false;    

    public override void PlayEvent()
    {
        Assert.IsTrue( null != _info, "Event info is not set");

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not set before termination");


        if (null != _eventCoroutine)
        {
            StopCoroutine( _eventCoroutine );
            _eventCoroutine = null;
        }

        // Todo: Release the event info
        // if (_info.IsFromAddressables) Addressables.Release(_info);
        // else Destroy(_info);
        _info = null;

        GameEventPool<ChoiceEvent>.Release( this );

        base.TerminateEvent();
    }

    public override GameEventInfo GetEventInfo() => _info;

    public override GameEventType GetEventType() => GameEventType.Choice;


    /****** Private Members ******/

    private Coroutine       _eventCoroutine     = null;
    private ChoiceEventInfo _info               = null;

    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue(null != _info, "Event info is not set");


        // Set choice info and switch to choice UI
        UIController.Instance.SetChoiceInfo(_info.ChoiceList);
        UIController.Instance.TurnSubUIOn(SubUI.Choice);

        // Wait for player to select a choice
        yield return new WaitUntil( () =>  null != UIController.Instance.GetSelectedChoice() ||
            false == GameEventManager.Instance.IsEventTypeActive(GameEventType.Story) );

        if (false == GameEventManager.Instance.IsEventTypeActive(GameEventType.Story))
        {
            Debug.LogWarning("ChoiceEvent is terminated due to the termination of StoryEvent.");
            TerminateEvent();
            yield break;
        }

        // Get the selected choice and process it
        string selectedChoice       = UIController.Instance.GetSelectedChoice();
        bool shouldGenerateResponse = _info.ChoiceList == null;

        StoryController.Instance.ProcessSelectedChoice(selectedChoice, shouldGenerateResponse);

        TerminateEvent();
    }
}