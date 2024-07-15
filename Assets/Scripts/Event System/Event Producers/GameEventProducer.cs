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

        //// Second, load stage scene asynchronously
        //SceneLoadEvent sceneLoadEvent = ScriptableObject.CreateInstance<SceneLoadEvent>();
        //sceneLoadEvent.sceneName = "StageScene";
        //dataLoadEvent.NextEvent = sceneLoadEvent;

        // Third, play prologue story
        StoryEvent storyEvent0 = ScriptableObject.CreateInstance<StoryEvent>();
        storyEvent0.stage = STAGE.EVENT;
        storyEvent0.storyNum = 0;
        storyEvent0.readBlockCount = storyEvent0.readEntryCount = 0;
        //sceneLoadEvent.NextEvent = storyEvent;
        dataLoadEvent.NextEvent = storyEvent0;

        // Third, play prologue story
        StoryEvent storyEvent1 = ScriptableObject.CreateInstance<StoryEvent>();
        storyEvent1.stage = STAGE.EVENT;
        storyEvent1.storyNum = 1;
        storyEvent1.readBlockCount = storyEvent1.readEntryCount = 0;
        storyEvent0.NextEvent = storyEvent1;

        // Third, play prologue story
        StoryEvent storyEvent2 = ScriptableObject.CreateInstance<StoryEvent>();
        storyEvent2.stage = STAGE.EVENT;
        storyEvent2.storyNum = 2;
        storyEvent2.readBlockCount = storyEvent2.readEntryCount = 0;
        storyEvent1.NextEvent = storyEvent2;

        //// Fourth, Activate scene
        //SceneActivateEvent sceneActivateEvent = ScriptableObject.CreateInstance<SceneActivateEvent>();
        //storyEvent.NextEvent = sceneActivateEvent;

        //// Fifth, auto save current user data
        //DataSaveEvent dataSaveEvent = ScriptableObject.CreateInstance<DataSaveEvent>();
        //dataSaveEvent.slotNum = 0;
        //sceneActivateEvent.NextEvent = dataSaveEvent;

        //// Sixth, play Cutscene event
        //CutsceneEvent cutsceneEvent = ScriptableObject.CreateInstance<CutsceneEvent>();
        //dataSaveEvent.NextEvent = cutsceneEvent;

        //// Finally, change ui to control ui
        //UIChangeEvent uiChangeEvent = ScriptableObject.CreateInstance<UIChangeEvent>();
        //uiChangeEvent.changingUI = BASEUI.CONTROL;
        //cutsceneEvent.NextEvent = uiChangeEvent;

        // Handle generated event stream
        HandleGeneratedEventChain(dataLoadEvent);
    }


    // Generate load game event stream. If there is no parameter, load recent game
    public void GenerateLoadGameEventStream(int slotNum = int.MaxValue)
    {

    }

    public void GenerateSaveGameEventStream(int slotNUm)
    {

    }

    // Generate show choice event stream
    public void GenerateShowChoiceEventStream()
    {
        ShowChoiceEvent showChoiceEvent = ScriptableObject.CreateInstance<ShowChoiceEvent>();

        HandleGeneratedEventChain(showChoiceEvent);
    }

    // Generate select choice event stream
    public void GenerateSelectChoiceEventStream(string text)
    {
        SelectChoiceEvent selectChoiceEvent = ScriptableObject.CreateInstance<SelectChoiceEvent>();
        selectChoiceEvent.optionText = text;

        HandleGeneratedEventChain(selectChoiceEvent);
    }

    // Handle generated game event to GameEventRouter
    private void HandleGeneratedEventChain(EventBase generated)
    {
        // If generated event is compatible, play event chian
        if (EventChecker.Instance.CheckEventCompatibility(generated))
        {
            GameEventManager.Instance.StartEventChain(generated);
        }
    }
}
