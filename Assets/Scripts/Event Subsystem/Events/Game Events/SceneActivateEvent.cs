using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIEnums;
using EventEnums;
using UnityEngine.Assertions;

/*
 * Activate loaded scene
 */

public class SceneActivateEvent : GameEventBase<SceneActivateEventInfo>
{
    /****** Public Members *****/

    public override bool            ShouldBeSaved   => false;
    public override GameEventInfo   EventInfo       => _info;
    public override GameEventType   EventType       => GameEventType.SceneActivate;

    public override void Initialize(SceneActivateEventInfo eventInfo, IGameEvent parentEvent = null)
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
        Assert.IsTrue(null != _info, "Event info is not set.");

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(false == SceneController.Instance.IsSceneLoading, "Should not be terminated when scene is not loaded yet.");
        Assert.IsTrue(null != _info, "Event info is not set before termination");

        if (_eventCoroutine != null)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        _info.DestroyInfo();
        _info = null;

        GameEventPool<SceneActivateEvent, SceneActivateEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private Coroutine               _eventCoroutine         = null;
    private SceneActivateEventInfo  _info                   = null;

    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue(null != _info, "Event info is not set.");


        // If scene is still loading
        if (SceneController.Instance.IsSceneLoading)
        {
            if (_info.ShouldTurnOnLoadingUI)
                UIController.Instance.ChangeBaseUI(BaseUI.Loading);

            yield return new WaitWhile(() => SceneController.Instance.IsSceneLoading);
        }

        // Initialize player character for game play
        Transform player = SceneController.Instance.FindPlayerTransform();
        if (player != null)
        {
            PlayerType character = PlayerManager.Instance.CurrentPlayerType;
            GamePlayManager.Instance.InitializePlayerCharacter(player, character);
        }

        SceneController.Instance.ActivateGameScene();

        TerminateEvent();
    }
}