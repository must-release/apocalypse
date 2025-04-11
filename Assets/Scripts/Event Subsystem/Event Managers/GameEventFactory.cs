using UnityEngine;
using System.Collections.Generic;
using StageEnums;
using EventEnums;
using SceneEnums;
using ScreenEffectEnums;
using UIEnums;
using UnityEngine.Assertions;

public static class GameEventFactory
{
    /****** Public Members ******/

    public static ChoiceEvent CreateChoiceEvent(List<string> choices)
    {
        var info = ScriptableObject.CreateInstance<ChoiceEventInfo>();
        info.Initialize(choices);

        var evt = GameEventPool<ChoiceEvent>.Get(EventHost, $"ChoiceEvent_{string.Join("_", choices)}");
        evt.SetEventInfo(info);
        return evt;
    }

    public static CutsceneEvent CreateCutsceneEvent()
    {
        var info = ScriptableObject.CreateInstance<CutsceneEventInfo>();
        info.Initialize();

        var evt = GameEventPool<CutsceneEvent>.Get(EventHost, "CutsceneEvent");
        evt.SetEventInfo(info);
        return evt;
    }

    public static DataLoadEvent CreateDataLoadEvent(int slotNum, bool isNewGame = false, bool isContinueGame = false)
    {
        var info = ScriptableObject.CreateInstance<DataLoadEventInfo>();
        info.Initialize(slotNum, isNewGame, isContinueGame);

        var evt = GameEventPool<DataLoadEvent>.Get(EventHost, $"DataLoadEvent_{slotNum}_{isNewGame}_{isContinueGame}");
        evt.SetEventInfo(info);
        return evt;
    }

    public static DataSaveEvent CreateDataSaveEvent(int slotNum)
    {
        var info = ScriptableObject.CreateInstance<DataSaveEventInfo>();
        info.Initialize(slotNum);

        var evt = GameEventPool<DataSaveEvent>.Get(EventHost, $"DataSaveEvent_{slotNum}");
        evt.SetEventInfo(info);
        return evt;
    }

    public static SceneActivateEvent CreateSceneActivateEvent()
    {
        var info = ScriptableObject.CreateInstance<SceneActivateEventInfo>();
        info.Initialize();

        var evt = GameEventPool<SceneActivateEvent>.Get(EventHost, "SceneActivateEvent");
        evt.SetEventInfo(info);
        return evt;
    }

    public static SceneLoadEvent CreateSceneLoadEvent(Scene loadingScene)
    {
        var info = ScriptableObject.CreateInstance<SceneLoadEventInfo>();
        info.Initialize(loadingScene);

        var evt = GameEventPool<SceneLoadEvent>.Get(EventHost, $"SceneLoadEvent_{loadingScene}");
        evt.SetEventInfo(info);
        return evt;
    }

    public static ScreenEffectEvent CreateScreenEffectEvent(ScreenEffect screenEffectType)
    {
        var info = ScriptableObject.CreateInstance<ScreenEffectEventInfo>();
        info.Initialize(screenEffectType);

        var evt = GameEventPool<ScreenEffectEvent>.Get(EventHost, $"ScreenEffectEvent_{screenEffectType}");
        evt.SetEventInfo(info);
        return evt;
    }

    public static StoryEvent CreateStoryEvent(Stage storyStage, int storyNumber, int readBlockCount, int readEntryCount, bool isOnMap)
    {
        var info = ScriptableObject.CreateInstance<StoryEventInfo>();
        info.Initialize(storyStage, storyNumber, readBlockCount, readEntryCount, isOnMap);

        var evt = GameEventPool<StoryEvent>.Get(EventHost, $"StoryEvent_{storyStage}_{storyNumber}");
        evt.SetEventInfo(info);
        return evt;
    }

    public static UIChangeEvent CreateUIChangeEvent(BaseUI targetUI)
    {
        var info = ScriptableObject.CreateInstance<UIChangeEventInfo>();
        info.Initialize(targetUI);

        var evt = GameEventPool<UIChangeEvent>.Get(EventHost, $"UIChangeEvent_{targetUI}");
        evt.SetEventInfo(info);
        return evt;
    }

    public static SequentialEvent CreateSequentialEvent(List<GameEvent> gameEvents)
    {
        Assert.IsTrue(null != gameEvents && 0 < gameEvents.Count, "GameEvents list is null or empty");

        var info = ScriptableObject.CreateInstance<SequentialEventInfo>();
        info.Initialize(new List<GameEventInfo>());

        var evt = GameEventPool<SequentialEvent>.Get(EventHost, "SequentialEvent");
        evt.SetEventInfo(info);

        foreach (var gameEvent in gameEvents)
        {
            evt.AddEvent(gameEvent);
        }

        return evt;
    }

    public static GameEvent CreateFromInfo(GameEventInfo info)
    {
        Assert.IsTrue((int)GameEventType.GameEventTypeCount == 12, "Don't forget to update CreateFromInfo when adding a new GameEventType.");

        switch (info.EventType)
        {
            case GameEventType.Story:
                var storyEvent = GameEventPool<StoryEvent>.Get(EventHost, "StoryEvent");
                storyEvent.SetEventInfo((StoryEventInfo)info);
                return storyEvent;
            case GameEventType.Sequential:
                var sequentialEvent = GameEventPool<SequentialEvent>.Get(EventHost, "SequentialEvent");
                sequentialEvent.SetEventInfo((SequentialEventInfo)info);
                return sequentialEvent;
            case GameEventType.Choice:
                var choiceEvent = GameEventPool<ChoiceEvent>.Get(EventHost, "ChoiceEvent");
                choiceEvent.SetEventInfo((ChoiceEventInfo)info);
                return choiceEvent;
            case GameEventType.Cutscene:
                var cutsceneEvent = GameEventPool<CutsceneEvent>.Get(EventHost, "CutsceneEvent");
                cutsceneEvent.SetEventInfo((CutsceneEventInfo)info);
                return cutsceneEvent;
            case GameEventType.DataLoad:
                var loadEvent = GameEventPool<DataLoadEvent>.Get(EventHost, "DataLoadEvent");
                loadEvent.SetEventInfo((DataLoadEventInfo)info);
                return loadEvent;
            case GameEventType.DataSave:
                var saveEvent = GameEventPool<DataSaveEvent>.Get(EventHost, "DataSaveEvent");
                saveEvent.SetEventInfo((DataSaveEventInfo)info);
                return saveEvent;
            case GameEventType.SceneActivate:
                var sceneActivate = GameEventPool<SceneActivateEvent>.Get(EventHost, "SceneActivateEvent");
                sceneActivate.SetEventInfo((SceneActivateEventInfo)info);
                return sceneActivate;
            case GameEventType.SceneLoad:
                var sceneLoad = GameEventPool<SceneLoadEvent>.Get(EventHost, "SceneLoadEvent");
                sceneLoad.SetEventInfo((SceneLoadEventInfo)info);
                return sceneLoad;
            case GameEventType.ScreenEffect:
                var screenEffect = GameEventPool<ScreenEffectEvent>.Get(EventHost, "ScreenEffectEvent");
                screenEffect.SetEventInfo((ScreenEffectEventInfo)info);
                return screenEffect;
            case GameEventType.UIChange:
                var uiChange = GameEventPool<UIChangeEvent>.Get(EventHost, "UIChangeEvent");
                uiChange.SetEventInfo((UIChangeEventInfo)info);
                return uiChange;
            default:
                Debug.LogError("Unknown GameEventType: " + info.EventType);
                return null;
        }
    }

    /****** Private Members *******/

    private static GameObject _eventHost;

    private static Transform EventHost
    {
        get
        {
            if (_eventHost == null)
            {
                _eventHost = GameObject.Find("Event System");
                if (_eventHost == null)
                    Debug.LogError("Event System not found in current scene.");
            }
            return _eventHost.transform;
        }
    }
}
