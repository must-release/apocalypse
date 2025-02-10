using UnityEngine;
using System.Collections;
using UIEnums;
using EventEnums;
using CharacterEums;

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
    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
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
        GameEventManager.Instance.StartCoroutine(PlayEventCoroutine());
    }
    public override IEnumerator PlayEventCoroutine()
    {
        // Succeed parent events
        if(parentEvent)
        {
            GameEventManager.Instance.SucceedParentEvents(ref parentEvent);
        }

        // If scene is still loading
        if (GameSceneController.Instance.IsSceneLoading)
        {
            // If it's not splash screen, change to Loading UI
            UIController.Instance.GetCurrentUI(out BaseUI baseUI, out _);
            if(baseUI != BaseUI.SplashScreen)
                UIController.Instance.ChangeBaseUI(BaseUI.Loading);

            // Wait for loading to end
            while (GameSceneController.Instance.IsSceneLoading)
            {
                yield return null;
            }
        }

        // Initialize player character for game play
        Transform player = GameSceneController.Instance.FindPlayerTransform();
        if(player != null)
        {
            PLAYER character = PlayerManager.Instance.Character;
            GamePlayManager.Instance.InitializePlayerCharacter(player, character);
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
            Debug.LogError("Terminate error: scene is not loaded yet!");
        }
    }
}
