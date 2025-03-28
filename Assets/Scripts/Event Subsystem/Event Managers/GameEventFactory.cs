using UnityEngine;
using System.Collections.Generic;
using StageEnums;
using EventEnums;
using SceneEnums


using ScreenEffectEnums;
using UIEnums;

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

        var evt = GameEventPool<UIChangeEvent>.Get(EventHost, $"UIChangeEvent{targetUI}");
        evt.SetEventInfo(info);
        return evt;
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
                    Debug.LogError("Event System not found");
            }
            return _eventHost.transform;
        }
    }
}
