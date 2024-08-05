using UnityEngine;
using UIEnums;
using EventEnums;
using SceneEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSceneLoad", menuName = "Event/SceneLoadEvent", order = 0)]
public class SceneLoadEvent : GameEvent
{
    private int loadingScene;
    public SCENE LoadingScene { get { return (SCENE)loadingScene; }  set { loadingScene = (int)value;  } }

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.SCENE_LOAD;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, BASEUI baseUI, SUBUI subUI)
    {
        // Can be played when current base UI is title, pause or load
        if (baseUI == BASEUI.TITLE || baseUI == BASEUI.SPLASH_SCREEN || subUI == SUBUI.PAUSE || subUI == SUBUI.LOAD )
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
        GameSceneController.Instance.LoadGameScene(LoadingScene);

        // Terminate scene load event and play next event
        GameEventManager.Instance.TerminateGameEvent(this);
    }
}

