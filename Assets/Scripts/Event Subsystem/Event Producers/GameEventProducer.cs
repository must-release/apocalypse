using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEnums;
using StageEnums;
using SceneEnums;
using ScreenEffectEnums;

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
        else
        {
            Destroy(this);
        }
    }

    public void Start()
    {
        // When game starts, play splash screen
        //StartCoroutine(GenerateSplashScreenEventStream());
    }

    // Generate splash screent event stream
    public void GenerateSplashScreenEventStream()
    {
        // First, set UI to splash scren UI
        UIChangeEvent changeUIEvent = ScriptableObject.CreateInstance<UIChangeEvent>();
        changeUIEvent.changingUI = BaseUI.SplashScreen;

        // Second, load title scene asynchronously
        SceneLoadEvent sceneLoadEvent = ScriptableObject.CreateInstance<SceneLoadEvent>();
        sceneLoadEvent.LoadingScene = Scene.Title;
        changeUIEvent.NextEvent = sceneLoadEvent;

        // Third, Activate scene
        SceneActivateEvent sceneActivateEvent = ScriptableObject.CreateInstance<SceneActivateEvent>();
        sceneLoadEvent.NextEvent = sceneActivateEvent;

        // Fourth, show fade out effect
        ScreenEffectEvent fadeOutEvent = ScriptableObject.CreateInstance<ScreenEffectEvent>();
        fadeOutEvent.screenEffect = ScreenEffect.FadeOut;
        sceneActivateEvent.NextEvent = fadeOutEvent;

        // Fifth, change UI to title UI
        UIChangeEvent uiChangeEvent = ScriptableObject.CreateInstance<UIChangeEvent>();
        uiChangeEvent.changingUI = BaseUI.Title;
        fadeOutEvent.NextEvent = uiChangeEvent;

        // Finall, show fade in effect
        ScreenEffectEvent fadeInEvent = ScriptableObject.CreateInstance<ScreenEffectEvent>();
        fadeInEvent.screenEffect = ScreenEffect.FadeIn;
        uiChangeEvent.NextEvent = fadeInEvent;

        // Handle generated event stream
        HandleGeneratedEventChain(changeUIEvent);
    }

    // Generate new game event stream
    public void GenerateNewGameEventStream()
    {
        // First, create user data(load initial data)
        DataLoadEvent dataLoadEvent = ScriptableObject.CreateInstance<DataLoadEvent>();
        dataLoadEvent.isNewGame = true;

        // Second, load stage scene asynchronously
        SceneLoadEvent sceneLoadEvent = ScriptableObject.CreateInstance<SceneLoadEvent>();
        sceneLoadEvent.LoadingScene = Scene.Stage;
        dataLoadEvent.NextEvent = sceneLoadEvent;

        // Third, play prologue story
        StoryEvent storyEvent = ScriptableObject.CreateInstance<StoryEvent>();
        storyEvent.stage = Stage.Prologue;
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
        uiChangeEvent.changingUI = BaseUI.Control;
        cutsceneEvent.NextEvent = uiChangeEvent;

        // Handle generated event stream
        HandleGeneratedEventChain(dataLoadEvent);
    }

    // Generate load game event stream. If there is no parameter, load recent game
    public void GenerateLoadGameEventStream(int slotNum = int.MaxValue)
    {
        // First, load data of the selected slot
        DataLoadEvent dataLoadEvent = ScriptableObject.CreateInstance<DataLoadEvent>();
        dataLoadEvent.slotNum = slotNum;
        if (slotNum == int.MaxValue)
        {
            dataLoadEvent.isContinueGame = true; // load recent game
        }

        // Second, load stage scene asynchronously
        SceneLoadEvent sceneLoadEvent = ScriptableObject.CreateInstance<SceneLoadEvent>();
        sceneLoadEvent.LoadingScene = Scene.Stage;
        dataLoadEvent.NextEvent = sceneLoadEvent;

        // Third, Activate scene 
        SceneActivateEvent sceneActivateEvent = ScriptableObject.CreateInstance<SceneActivateEvent>();
        sceneLoadEvent.NextEvent = sceneActivateEvent;

        // Handle generated event stream
        HandleGeneratedEventChain(dataLoadEvent);
    }

    // Generate scene change event. Load certain scene and change it to current scene
    public void GenerateChangeSceneEventStream(Scene scene)
    {
        // First, load stage scene asynchronously
        SceneLoadEvent sceneLoadEvent = ScriptableObject.CreateInstance<SceneLoadEvent>();
        sceneLoadEvent.LoadingScene = scene;

        // Second, Activate scene 
        SceneActivateEvent sceneActivateEvent = ScriptableObject.CreateInstance<SceneActivateEvent>();
        sceneLoadEvent.NextEvent = sceneActivateEvent;

        // Third, Change UI according to scene
        UIChangeEvent changeUIEvent = ScriptableObject.CreateInstance<UIChangeEvent>();
        changeUIEvent.changingUI = scene == Scene.Title? BaseUI.Title : BaseUI.Control;
        sceneActivateEvent.NextEvent = changeUIEvent;

        // Handle generated event stream
        HandleGeneratedEventChain(sceneLoadEvent);
    }

    // Generate save event stream
    public void GenerateSaveGameEventStream(int slotNum)
    {
        DataSaveEvent dataSaveEvent = ScriptableObject.CreateInstance<DataSaveEvent>();
        dataSaveEvent.slotNum = slotNum;

        HandleGeneratedEventChain(dataSaveEvent);
    }

    // Generate choice event stream
    public void GenerateChoiceEventStream(List<string> choiceList = null)
    {
        ChoiceEvent choiceEvent = ScriptableObject.CreateInstance<ChoiceEvent>();
        choiceEvent.choiceList = choiceList;

        HandleGeneratedEventChain(choiceEvent);
    }

    public void GenerateGameOverEventStream()
    {
        UIChangeEvent changeUIEvent = ScriptableObject.CreateInstance<UIChangeEvent>();
        changeUIEvent.changingUI = BaseUI.GameOver;

        HandleGeneratedEventChain(changeUIEvent);
    }

    // Handle generated game event to GameEventManager
    private void HandleGeneratedEventChain(GameEvent generated)
    {
        // If generated event is compatible, play event chian
        if (EventChecker.Instance.CheckEventCompatibility(generated))
        {
            GameEventManager.Instance.StartEventChain(generated);
        }
    }
}
