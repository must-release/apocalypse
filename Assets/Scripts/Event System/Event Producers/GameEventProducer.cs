using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEnums;
using StageEnums;

/*
 * EventProducer which creates the game event stream
 */

public class GameEventProducer : MonoBehaviour
{
    public static GameEventProducer Instance;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Generate new game event stream
    public void GenerateNewGameEventStream()
    {
        // First, create user data(load initial data)
        DataLoadEvent dataLoadEvent = ScriptableObject.CreateInstance<DataLoadEvent>();
        dataLoadEvent.isNewGame = true;

        // Second, load stage scene asynchronously
        SceneLoadEvent sceneLoadEvent = ScriptableObject.CreateInstance<SceneLoadEvent>();
        sceneLoadEvent.sceneName = "StageScene";
        dataLoadEvent.NextEvent = sceneLoadEvent;

        // Third, play prologue story
        StoryEvent storyEvent = ScriptableObject.CreateInstance<StoryEvent>();
        storyEvent.stage = STAGE.PROLOGUE;
        storyEvent.storyNum = 0;
        storyEvent.readBlockCount = storyEvent.readEntryCount = 0;
        sceneLoadEvent.NextEvent = storyEvent;

        // Fourth, Activate scene
        SceneActivateEvent sceneActivateEvent = ScriptableObject.CreateInstance<SceneActivateEvent>();
        storyEvent.NextEvent = sceneActivateEvent;

        // Fifth, auto save current user data
        DataSaveEvent dataSaveEvent = ScriptableObject.CreateInstance<DataSaveEvent>();
        dataSaveEvent.slotNum = 0;
        sceneActivateEvent.NextEvent = dataSaveEvent;

        // Sixth, play Cutscene event
        CutsceneEvent cutsceneEvent = ScriptableObject.CreateInstance<CutsceneEvent>();
        dataSaveEvent.NextEvent = cutsceneEvent;

        // Finally, change ui to control ui
        UIChangeEvent uiChangeEvent = ScriptableObject.CreateInstance<UIChangeEvent>();
        uiChangeEvent.changingUI = BASEUI.CONTROL;
        cutsceneEvent.NextEvent = uiChangeEvent;

        // Handle generated event stream
        HandleGeneratedEventChain(dataLoadEvent);
    }


    // Generate load game event stream. If there is no parameter, load recent game
    public void GenerateLoadGameEventStream(int slotNum = int.MaxValue)
    {
        // First, load data of the selected slot
        DataLoadEvent dataLoadEvent = ScriptableObject.CreateInstance<DataLoadEvent>();
        if(slotNum == int.MaxValue)
        {
            dataLoadEvent.isContinueGame = true; // load recent game
        }

        // Second, load stage scene asynchronously
        SceneLoadEvent sceneLoadEvent = ScriptableObject.CreateInstance<SceneLoadEvent>();
        sceneLoadEvent.sceneName = "StageScene";
        dataLoadEvent.NextEvent = sceneLoadEvent;

        // Third, Activate scene
        SceneActivateEvent sceneActivateEvent = ScriptableObject.CreateInstance<SceneActivateEvent>();
        sceneLoadEvent.NextEvent = sceneActivateEvent;

        // Handle generated event stream
        HandleGeneratedEventChain(dataLoadEvent);
    }


    // Generate save event stream
    public void GenerateSaveGameEventStream(int slotNum)
    {
        DataSaveEvent dataSaveEvent = ScriptableObject.CreateInstance<DataSaveEvent>();
        dataSaveEvent.slotNum = slotNum;

        HandleGeneratedEventChain(dataSaveEvent);
    }

    // Generate choice event stream
    public void GenerateChoiceEventStream(List<string> choiceList)
    {
        ChoiceEvent choiceEvent = ScriptableObject.CreateInstance<ChoiceEvent>();
        choiceEvent.choiceList = choiceList;

        HandleGeneratedEventChain(choiceEvent);
    }

    // Handle generated game event to GameEventManager
    private void HandleGeneratedEventChain(EventBase generated)
    {
        // If generated event is compatible, play event chian
        if (EventChecker.Instance.CheckEventCompatibility(generated))
        {
            GameEventManager.Instance.StartEventChain(generated);
        }
    }
}
