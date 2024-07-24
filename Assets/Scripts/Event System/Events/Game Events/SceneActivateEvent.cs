using UnityEngine;
using System.Collections;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSceneActivate", menuName = "Event/SceneActivateEvent", order = 0)]
public class SceneActivateEvent : GameEvent
{
    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.SCENE_ACTIVATE;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, (BASEUI, SUBUI) currentUI)
    {
        // Can be played when current base UI is loading or save
        if (parentEvent == null || parentEvent.EventType == EVENT_TYPE.STORY || parentEvent.EventType == EVENT_TYPE.CHOICE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Play scene activate event
    public override void PlayEvent()
    {
        // Use GameEventManger to start coroutine
        GameEventManager.Instance.StartCoroutineForGameEvents(PlayEventCoroutine());
    }
    IEnumerator PlayEventCoroutine()
    {
        // Succeed parent events
        if(ParentEvent)
        {
            GameEventManager.Instance.SucceedParentEvents(ParentEvent);
        }

        // If scene is already loaded
        if (!GameSceneController.Instance.IsSceneLoading)
        {
            GameSceneController.Instance.ActivateGameScene();
            yield break;
        }

        // If it's not splash screen, change to Loading UI
        if(UIController.Instance.GetCurrentUI().Item1 != BASEUI.SPLASH_SCREEN)
            UIController.Instance.ChangeBaseUI(BASEUI.LOADING);

        // Wait for loading to end
        while (GameSceneController.Instance.IsSceneLoading)
        {
            yield return null;
        }

        // Activate game scene
        GameSceneController.Instance.ActivateGameScene();

        // Terminate scene activate event and play next event
        GameEventManager.Instance.TerminateGameEvent(this);
    }

    // Terminate scene activate event
    public override void TerminateEvent()
    {
        if (GameSceneController.Instance.IsSceneLoading)
        {
            Debug.Log("Terminate error: scene is not loaded yet!");
        }
    }
}
