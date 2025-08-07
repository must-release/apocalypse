using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*
 * 스토리 진행 중 화면에 선택지를 표시하고, 플레이어가 고른 선택지를 처리하는 ChoiceEvent입니다.
 */

public class ChoiceEvent : GameEventBase<ChoiceEventInfo>
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => false;
    public override GameEventType   EventType       => GameEventType.Choice;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Debug.Assert(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        if (activeEventTypeCounts.ContainsKey(GameEventType.Story))
            return true;
        
        return false;
    }

    public override void PlayEvent()
    {
        Debug.Assert( null != Info, "Event info is not set");

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");


        if (null != _eventCoroutine)
        {
            StopCoroutine( _eventCoroutine );
            _eventCoroutine = null;
        }

        Info.DestroyInfo();
        Info = null;

        GameEventPool<ChoiceEvent, ChoiceEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private Coroutine       _eventCoroutine     = null;

    private IEnumerator PlayEventCoroutine()
    {
        Debug.Assert(null != Info, "Event info is not set");


        // Set choice info and switch to choice UI
        UIController.Instance.SetChoiceInfo(Info.ChoiceList);
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
        string selectedChoice = UIController.Instance.GetSelectedChoice();

        AD.Story.StoryController.Instance.ProcessSelectedChoice(selectedChoice);

        TerminateEvent();
    }
}