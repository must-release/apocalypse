using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * EventHandler which creates the game event stream
 */

public class GameEventHandler : MonoBehaviour
{
    public static GameEventHandler Instance;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Generate new game event stream
    public void HandleNewGameEvent()
    {
        // First, create user data(load initial data)
        DataLoadEvent dataLoadEvent = ScriptableObject.CreateInstance<DataLoadEvent>();
        dataLoadEvent.slotNum = -1;

        // Second, load stage scene asynchronously
        SceneLoadEvent sceneLoadEvent = ScriptableObject.CreateInstance<SceneLoadEvent>();
        dataLoadEvent.NextEvent = sceneLoadEvent;

        // Third, play prologue story
        StoryEvent storyEvent = ScriptableObject.CreateInstance<StoryEvent>();
        storyEvent.stage = UserData.STAGE.TUTORIAL;
        storyEvent.storyNum = 0;
        sceneLoadEvent.NextEvent = storyEvent;

        // Fourth, play loading
        LoadingEvent loadingEvent = ScriptableObject.CreateInstance<LoadingEvent>();
        storyEvent.NextEvent = loadingEvent;

        // Fifth, auto save current user data
        DataSaveEvent dataSaveEvent = ScriptableObject.CreateInstance<DataSaveEvent>();
        dataSaveEvent.slotNum = 0;
        loadingEvent.NextEvent = dataSaveEvent;

        // Sixth, play in-game event
        InGameEvent inGameEvent = ScriptableObject.CreateInstance<InGameEvent>();
        dataSaveEvent.NextEvent = inGameEvent;

        // Finally, change ui to control ui
        UIChangeEvent uiChangeEvent = ScriptableObject.CreateInstance<UIChangeEvent>();

        // Handle generated event stream
        GameEventRouter.Instance.PlayEvent(dataLoadEvent);
    }
}
