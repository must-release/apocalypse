using UnityEngine;
using System.Collections;
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
        Assert.IsTrue(eventInfo.IsInitialized, "Event info is not initialized");

        _sceneActivateEventInfo = eventInfo;
    }

    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
    {
        if (null == parentEvent || GameEventType.Story == parentEvent.EventType || GameEventType.Choice == parentEvent.EventType)
            return true;

        return false;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != _sceneActivateEventInfo, "Event info is not set.");

        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override GameEventInfo GetEventInfo()
    {
        return _sceneActivateEventInfo;
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(GameSceneController.Instance.IsSceneLoading, "Should not be terminated when scene is not loaded yet.");
        Assert.IsTrue(null != _sceneActivateEventInfo, "Event info is not set before termination");

        if (_eventCoroutine != null)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        ScriptableObject.Destroy(_sceneActivateEventInfo);
        _sceneActivateEventInfo = null;

        GameEventPool<SceneActivateEvent>.Release(this);
    }


    /****** Private Members ******/

    private Coroutine               _eventCoroutine         = null;
    private SceneActivateEventInfo  _sceneActivateEventInfo = null;

    // TODO : Move SucceedParentEvents function to GameEventManager
    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue(null != _sceneActivateEventInfo, "Event info is not set.");

        // Succeed parent events
        if (ParentEvent)
        {
            GameEventManager.Instance.SucceedParentEvents(ParentEvent);
            ParentEvent = null;
        }

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
        GameEventManager.Instance.TerminateGameEvent(this);
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