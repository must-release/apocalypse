﻿using UnityEngine;
using System.Collections.Generic;
using UIEnums;
using EventEnums;
using System.Collections;
using UnityEngine.Assertions;

/*
 * 스토리 진행 중 화면에 선택지를 표시하고, 플레이어가 고른 선택지를 처리하는 ChoiceEvent입니다.
 */

public class ChoiceEvent : GameEvent
{
    /****** Public Members ******/

    public void SetEventInfo(ChoiceEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not initialized");

        _info   = eventInfo;
        Status  = EventStatus.Waiting;
    }

    public override bool CheckCompatibility()
    {
        if (GameEventManager.Instance.IsEventTypeActive(GameEventType.Story))
            return true;
        
        return false;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue( null != _info, "Event info is not set");

        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    protected override void TerminateEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not set before termination");


        if ( null != _eventCoroutine )
        {
            StopCoroutine( _eventCoroutine );
            _eventCoroutine = null;
        }

        ScriptableObject.Destroy( _info );
        _info = null;

        GameEventPool<ChoiceEvent>.Release( this );

        base.TerminateEvent();
    }

    public override GameEventInfo GetEventInfo()
    {
        return _info;
    }


    /****** Private Members ******/

    private Coroutine       _eventCoroutine     = null;
    private ChoiceEventInfo _info    = null;

    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue(null != _info, "Event info is not set");


        // Set choice info and switch to choice UI
        UIController.Instance.SetChoiceInfo(_info.ChoiceList);
        UIController.Instance.TurnSubUIOn(SubUI.Choice);

        // Wait for player to select a choice
        yield return new WaitUntil( () =>  null != UIController.Instance.GetSelectedChoice() ||
            false == GameEventManager.Instance.IsEventTypeActive(GameEventType.Story) );

        if (GameEventManager.Instance.IsEventTypeActive(GameEventType.Story))
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

[CreateAssetMenu(fileName = "NewChoiceEvent", menuName = "EventInfo/ChoiceEvent", order = 0)]
public class ChoiceEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public List<string> ChoiceList { get { return _choiceList; } private set { _choiceList = value; }}
    
    public void Initialize(List<string> choices)
    {
        Assert.IsTrue( false == IsInitialized,  "Duplicate initialization of GameEventInfo is not allowed." );
        Assert.IsTrue(null != choices,          "Choices cannot be null.");
        Assert.IsTrue(choices.Count > 0,        "Choice list must have at least one item.");

        ChoiceList      = choices;
        IsInitialized   = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Choice;
    }

    protected override void OnValidate()
    {
        if ( null != ChoiceList && 0 < ChoiceList.Count )
            IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private List<string> _choiceList;
}