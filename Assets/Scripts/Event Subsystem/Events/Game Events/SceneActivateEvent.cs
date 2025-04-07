using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIEnums;
using EventEnums;
using CharacterEums;
using UnityEngine.Assertions;

/*
 * Activate loaded scene
 */

public class SceneActivateEvent : GameEvent
{
    /****** Public Members *****/

    public void SetEventInfo(SceneActivateEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info   = eventInfo;
        Status  = EventStatus.Waiting;
    }

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        foreach (GameEventType eventType in activeEventTypeCounts.Keys)
        {
            if (GameEventType.Story == eventType || GameEventType.Choice == eventType)
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

    public override GameEventInfo GetEventInfo()
    {
        return _info;
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(GameSceneController.Instance.IsSceneLoading, "Should not be terminated when scene is not loaded yet.");
        Assert.IsTrue(null != _info, "Event info is not set before termination");

        if (_eventCoroutine != null)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        ScriptableObject.Destroy(_info);
        _info = null;

        GameEventPool<SceneActivateEvent>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private Coroutine               _eventCoroutine         = null;
    private SceneActivateEventInfo  _info = null;

    // TODO : Move SucceedParentEvents function to GameEventManager
    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue(null != _info, "Event info is not set.");

        // Succeed parent events
        // if (ParentEvent)
        // {
        //     GameEventManager.Instance.SucceedParentEvents(ParentEvent);
        //     ParentEvent = null;
        // }

        // If scene is still loading
        if (GameSceneController.Instance.IsSceneLoading)
        {
            // If it's not splash screen, change to Loading UI
            UIController.Instance.GetCurrentUI(out BaseUI baseUI, out _);
            if (BaseUI.SplashScreen != baseUI)
                UIController.Instance.ChangeBaseUI(BaseUI.Loading);

            // Wait for loading to end
            yield return new WaitWhile(() => GameSceneController.Instance.IsSceneLoading);
        }

        // Initialize player character for game play
        Transform player = GameSceneController.Instance.FindPlayerTransform();
        if (player != null)
        {
            PLAYER character = PlayerManager.Instance.Character;
            GamePlayManager.Instance.InitializePlayerCharacter(player, character);
        }

        // Activate game scene
        GameSceneController.Instance.ActivateGameScene();

        // Terminate scene activate event and play next event
        TerminateEvent();
    }
}


[CreateAssetMenu(fileName = "NewSceneActivateEvent", menuName = "EventInfo/SceneActivateEvent", order = 0)]
public class SceneActivateEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public void Initialize()
    {
        Assert.IsTrue(false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed.");

        IsInitialized = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.SceneActivate;
    }

    protected override void OnValidate()
    {
        IsInitialized = true;
    }


    /****** Private Members ******/
}