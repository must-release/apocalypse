using EventEnums;
using SceneEnums;
using ScreenEffectEnums;
using StageEnums;
using System;
using System.Collections.Generic;
using UIEnums;
using UnityEngine;
using UnityEngine.Assertions;

public static class GameEventFactory
{
    /****** Public Members ******/

    public static IGameEvent CreateChoiceEvent(List<string> choices)
    {
        var info = ScriptableObject.CreateInstance<ChoiceEventInfo>();
        info.Initialize(choices);

        var evt = GameEventPool<ChoiceEvent, ChoiceEventInfo>.Get(EventHost, $"ChoiceEvent_{string.Join("_", choices)}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateCutsceneEvent()
    {
        var info = ScriptableObject.CreateInstance<CutsceneEventInfo>();
        info.Initialize();

        var evt = GameEventPool<CutsceneEvent, CutsceneEventInfo>.Get(EventHost, "CutsceneEvent");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateDataLoadEvent(int slotNum, bool isNewGame = false, bool isContinueGame = false)
    {
        var info = ScriptableObject.CreateInstance<DataLoadEventInfo>();
        info.Initialize(slotNum, isNewGame, isContinueGame);

        var evt = GameEventPool<DataLoadEvent, DataLoadEventInfo>.Get(EventHost, $"DataLoadEvent_{slotNum}_{isNewGame}_{isContinueGame}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateDataSaveEvent(int slotNum)
    {
        var info = ScriptableObject.CreateInstance<DataSaveEventInfo>();
        info.Initialize(slotNum);

        var evt = GameEventPool<DataSaveEvent, DataSaveEventInfo>.Get(EventHost, $"DataSaveEvent_{slotNum}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateSceneActivateEvent(bool ShouldTurnOnLoadingUI = true)
    {
        var info = ScriptableObject.CreateInstance<SceneActivateEventInfo>();
        info.Initialize(ShouldTurnOnLoadingUI);

        var evt = GameEventPool<SceneActivateEvent, SceneActivateEventInfo>.Get(EventHost, "SceneActivateEvent");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateSceneLoadEvent(SceneName loadingScene)
    {
        var info = ScriptableObject.CreateInstance<SceneLoadEventInfo>();
        info.Initialize(loadingScene);

        var evt = GameEventPool<SceneLoadEvent, SceneLoadEventInfo>.Get(EventHost, $"SceneLoadEvent_{loadingScene}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateScreenEffectEvent(ScreenEffect screenEffectType)
    {
        var info = ScriptableObject.CreateInstance<ScreenEffectEventInfo>();
        info.Initialize(screenEffectType);

        var evt = GameEventPool<ScreenEffectEvent, ScreenEffectEventInfo>.Get(EventHost, $"ScreenEffectEvent_{screenEffectType}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateStoryEvent(Stage storyStage, int storyNumber, int readBlockCount, int readEntryCount, bool isOnMap)
    {
        var info = ScriptableObject.CreateInstance<StoryEventInfo>();
        info.Initialize(storyStage, storyNumber, readBlockCount, readEntryCount, isOnMap);

        var evt = GameEventPool<StoryEvent, StoryEventInfo>.Get(EventHost, $"StoryEvent_{storyStage}_{storyNumber}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateUIChangeEvent(BaseUI targetUI)
    {
        var info = ScriptableObject.CreateInstance<UIChangeEventInfo>();
        info.Initialize(targetUI);

        var evt = GameEventPool<UIChangeEvent, UIChangeEventInfo>.Get(EventHost, $"UIChangeEvent_{targetUI}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateSequentialEvent(List<IGameEvent> gameEvents)
    {
        Assert.IsTrue(null != gameEvents && 0 < gameEvents.Count, "GameEvents list is null or empty");

        var info = ScriptableObject.CreateInstance<SequentialEventInfo>();
        info.Initialize(new List<GameEventInfo>());

        SequentialEvent evt = GameEventPool<SequentialEvent, SequentialEventInfo>.Get(EventHost, "SequentialEvent");
        evt.Initialize(info);

        foreach (var gameEvent in gameEvents)
        {
            evt.AddEvent(gameEvent);
        }

        return evt;
    }

    public static IGameEvent CreateFromInfo<TEventInfo>(TEventInfo info, IGameEvent parentEvent = null)
        where TEventInfo : GameEventInfo
    {
        Assert.IsTrue(null != info, "Cannot create event from null info");
        Assert.IsTrue(true == _eventCreators.ContainsKey(info.EventType), $"No factory registered for GameEventType: {info.EventType}");

        return _eventCreators[info.EventType].Invoke(info, parentEvent);
    }

    /****** Private Members *******/

    private static GameObject _eventHost;

    private static Transform EventHost
    {
        get
        {
            if (null == _eventHost)
            {
                _eventHost = GameObject.Find("Event System");
                if (null == _eventHost)
                    Debug.LogError("Event System not found in current scene.");
            }
            return _eventHost.transform;
        }
    }

    private static readonly Dictionary<GameEventType, Func<GameEventInfo, IGameEvent, IGameEvent>> _eventCreators =
        new Dictionary<GameEventType, Func<GameEventInfo, IGameEvent, IGameEvent>>
    {
        { GameEventType.Choice,         (info, parentEvent) => CreateFromInfo<ChoiceEvent, ChoiceEventInfo>(info as ChoiceEventInfo, parentEvent) },
        { GameEventType.Cutscene,       (info, parentEvent) => CreateFromInfo<CutsceneEvent, CutsceneEventInfo>(info as CutsceneEventInfo, parentEvent) },
        { GameEventType.DataLoad,       (info, parentEvent) => CreateFromInfo<DataLoadEvent,DataLoadEventInfo>(info as DataLoadEventInfo, parentEvent) },
        { GameEventType.DataSave,       (info, parentEvent) => CreateFromInfo<DataSaveEvent, DataSaveEventInfo>(info as DataSaveEventInfo, parentEvent) },
        { GameEventType.SceneActivate,  (info, parentEvent) => CreateFromInfo<SceneActivateEvent, SceneActivateEventInfo>(info as SceneActivateEventInfo, parentEvent) },
        { GameEventType.SceneLoad,      (info, parentEvent) => CreateFromInfo<SceneLoadEvent, SceneLoadEventInfo>(info as SceneLoadEventInfo, parentEvent) },
        { GameEventType.ScreenEffect,   (info, parentEvent) => CreateFromInfo<ScreenEffectEvent, ScreenEffectEventInfo>(info as ScreenEffectEventInfo, parentEvent) },
        { GameEventType.Sequential,     (info, parentEvent) => CreateFromInfo<SequentialEvent, SequentialEventInfo>(info as SequentialEventInfo, parentEvent) },
        { GameEventType.Story,          (info, parentEvent) => CreateFromInfo<StoryEvent, StoryEventInfo>(info as StoryEventInfo, parentEvent) },
        { GameEventType.UIChange,       (info, parentEvent) => CreateFromInfo<UIChangeEvent, UIChangeEventInfo>(info as UIChangeEventInfo, parentEvent) },
    };

    private static IGameEvent CreateFromInfo<TEvent, TInfo>(TInfo info, IGameEvent parentEvent)
        where TEvent : GameEventBase<TInfo>
        where TInfo : GameEventInfo
    {
        Assert.IsTrue(null != info, "Cannot create Event from null info");

        var gameEvent = GameEventPool<TEvent, TInfo>.Get(EventHost, info.EventType.ToString());
        gameEvent.Initialize(info, parentEvent);

        return gameEvent;
    }
}
