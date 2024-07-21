using UnityEngine;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSceneLoad", menuName = "Event/SceneLoadEvent", order = 0)]
public class SceneLoadEvent : GameEvent
{
    public string sceneName; // if null, load scene according to user data

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.SCENE_LOAD;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, (BASEUI, SUBUI) currentUI)
    {
        // Can be played when current base UI is title, pause or load
        if (currentUI.Item1 == BASEUI.TITLE || currentUI.Item2 == SUBUI.PAUSE || currentUI.Item2 == SUBUI.LOAD )
        {
            return true;
        }
        else
            return false;
    }

    // Play Scene Load Event
    public override void PlayEvent()
    {
        // Load scene asynchronously
        GameSceneController.Instance.LoadGameScene(sceneName);

        // Terminate scene load event and play next event
        GameEventManager.Instance.TerminateGameEvent(this);
    }
}

