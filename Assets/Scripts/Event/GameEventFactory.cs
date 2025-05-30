using SceneEnums;
using ScreenEffectEnums;
using StageEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class GameEventFactory
{
    /****** Public Members ******/

    public static IEnumerator LoadCommonGameEvents()
    {
        Assert.IsTrue(0 == _commonEventInfos.Count, "Duplicate Common event load.");

        var handle = Addressables.LoadAssetAsync<CommonEventAsset>(AssetPath.CommonEventAsset);
        yield return handle;

        if (AsyncOperationStatus.Failed == handle.Status)
        {
            Logger.Write(LogCategory.AssetLoad, "Failed to Load CommonEventAsset.");
            yield break;
        }

        foreach (var commonEventEntry in handle.Result.CommonEventAssets)
        {
            _commonEventInfos.Add(commonEventEntry.commonEventType, commonEventEntry.CommonEvent);
        }
    }

    public static IGameEvent CreateCommonEvent(CommonEventType commonEventType)
    {
        var eventInfo = _commonEventInfos[commonEventType].Clone();
        return CreateFromInfo(eventInfo);
    }

    public static IGameEvent CreateChoiceEvent(List<string> choices)
    {
        var info = ScriptableObject.CreateInstance<ChoiceEventInfo>();
        info.Initialize(choices);

        var evt = GameEventPool<ChoiceEvent, ChoiceEventInfo>.Get(EventHost, $"ChoiceEvent_{string.Join("_", choices)}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateChoiceEvent(ChoiceEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create ChoiceEvent from null DTO");

        return CreateChoiceEvent(dto.ChoiceList);
    }

    public static IGameEvent CreateCutsceneEvent()
    {
        var info = ScriptableObject.CreateInstance<CutsceneEventInfo>();
        info.Initialize();

        var evt = GameEventPool<CutsceneEvent, CutsceneEventInfo>.Get(EventHost, "CutsceneEvent");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateCutsceneEvent(CutsceneEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create DataLoadEvent from null DTO");
        return CreateCutsceneEvent();
    }

    public static IGameEvent CreateDataLoadEvent(int slotNum, bool isNewGame = false, bool isContinueGame = false)
    {
        var info = ScriptableObject.CreateInstance<DataLoadEventInfo>();
        info.Initialize(slotNum, isNewGame, isContinueGame);

        var evt = GameEventPool<DataLoadEvent, DataLoadEventInfo>.Get(EventHost, $"DataLoadEvent_{slotNum}_{isNewGame}_{isContinueGame}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateDataLoadEvent(DataLoadEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create DataSaveEvent from null DTO");
        return CreateDataLoadEvent(dto.SlotNum, dto.IsNewGame, dto.IsContinueGame);
    }

    public static IGameEvent CreateDataSaveEvent(int slotNum)
    {
        var info = ScriptableObject.CreateInstance<DataSaveEventInfo>();
        info.Initialize(slotNum);

        var evt = GameEventPool<DataSaveEvent, DataSaveEventInfo>.Get(EventHost, $"DataSaveEvent_{slotNum}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateDataSaveEvent(DataSaveEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create DataSaveEvent from null DTO");
        return CreateDataSaveEvent(dto.SlotNum);
    }

    public static IGameEvent CreateSceneActivateEvent(bool ShouldTurnOnLoadingUI = true)
    {
        var info = ScriptableObject.CreateInstance<SceneActivateEventInfo>();
        info.Initialize(ShouldTurnOnLoadingUI);

        var evt = GameEventPool<SceneActivateEvent, SceneActivateEventInfo>.Get(EventHost, "SceneActivateEvent");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateSceneActivateEvent(SceneActivateEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create SceneActivateEvent from null DTO");
        return CreateSceneActivateEvent(dto.ShouldTurnOnLoadingUI);
    }

    public static IGameEvent CreateSceneLoadEvent(SceneName loadingScene)
    {
        var info = ScriptableObject.CreateInstance<SceneLoadEventInfo>();
        info.Initialize(loadingScene);

        var evt = GameEventPool<SceneLoadEvent, SceneLoadEventInfo>.Get(EventHost, $"SceneLoadEvent_{loadingScene}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateSceneLoadEvent(SceneLoadEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create SceneLoadEvent from null DTO");
        return CreateSceneLoadEvent(dto.LoadingScene);
    }

    public static IGameEvent CreateScreenEffectEvent(ScreenEffect screenEffectType)
    {
        var info = ScriptableObject.CreateInstance<ScreenEffectEventInfo>();
        info.Initialize(screenEffectType);

        var evt = GameEventPool<ScreenEffectEvent, ScreenEffectEventInfo>.Get(EventHost, $"ScreenEffectEvent_{screenEffectType}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateScreenEffectEvent(ScreenEffectEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create ScreenEffectEvent from null DTO");
        return CreateScreenEffectEvent(dto.ScreenEffectType);
    }

    public static IGameEvent CreateStoryEvent(Stage storyStage, int storyNumber, int readBlockCount, int readEntryCount, bool isOnMap)
    {
        var info = ScriptableObject.CreateInstance<StoryEventInfo>();
        info.Initialize(storyStage, storyNumber, readBlockCount, readEntryCount, isOnMap);

        var evt = GameEventPool<StoryEvent, StoryEventInfo>.Get(EventHost, $"StoryEvent_{storyStage}_{storyNumber}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateStoryEvent(StoryEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create StoryEvent from null DTO");
        return CreateStoryEvent(dto.StoryStage, dto.StoryNumber, dto.ReadBlockCount, dto.ReadEntryCount, dto.IsOnMap);
    }

    public static IGameEvent CreateUIChangeEvent(BaseUI targetUI)
    {
        var info = ScriptableObject.CreateInstance<UIChangeEventInfo>();
        info.Initialize(targetUI);

        var evt = GameEventPool<UIChangeEvent, UIChangeEventInfo>.Get(EventHost, $"UIChangeEvent_{targetUI}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateUIChangeEvent(UIChangeEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create UIChangeEvent from null DTO");
        return CreateUIChangeEvent(dto.TargetUI);
    }

    public static IGameEvent CreateSequentialEvent(List<IGameEvent> gameEvents, int startIndex = 0)
    {
        Assert.IsTrue(null != gameEvents && 0 < gameEvents.Count, "GameEvents list is null or empty");

        var info = ScriptableObject.CreateInstance<SequentialEventInfo>();
        info.Initialize(new List<GameEventInfo>(), startIndex);

        SequentialEvent evt = GameEventPool<SequentialEvent, SequentialEventInfo>.Get(EventHost, "SequentialEvent");
        evt.Initialize(info);

        foreach (var gameEvent in gameEvents)
        {
            evt.AddNewEvent(gameEvent);
        }

        return evt;
    }

    public static IGameEvent CreateSequentialEvent(SequentialEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create SequentialEvent from null DTO");

        var eventList = new List<IGameEvent>();

        foreach (var eventDTO in dto.EventDTOs)
        {
            eventList.Add(CreateFromDTO(eventDTO));
        }

        return CreateSequentialEvent(eventList, dto.StartIndex);
    }

    public static IGameEvent CreateFromInfo<TEventInfo>(TEventInfo info)
        where TEventInfo : GameEventInfo
    {
        Assert.IsTrue(null != info, "Cannot create event from null info");
        Assert.IsTrue(true == _eventCreatorsFromInfo.ContainsKey(info.EventType), $"No info creator registered for GameEventType: {info.EventType}");

        return _eventCreatorsFromInfo[info.EventType].Invoke(info);
    }

    public static IGameEvent CreateFromDTO(GameEventDTO dto)
    {
        Assert.IsTrue(null != dto, "Cannot create event from null DTO");
        Assert.IsTrue(true == _eventCreatorsFromDTO.ContainsKey(dto.EventType), $"No dto creator registered for GameEventType: {dto.EventType}");

        return _eventCreatorsFromDTO[dto.EventType].Invoke(dto);
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

    private static readonly Dictionary<CommonEventType, GameEventInfo> _commonEventInfos = new();

    private static readonly Dictionary<GameEventType, Func<GameEventInfo, IGameEvent>> _eventCreatorsFromInfo =
        new Dictionary<GameEventType, Func<GameEventInfo, IGameEvent>>
    {
        { GameEventType.Choice,         info => CreateFromInfo<ChoiceEvent, ChoiceEventInfo>(info as ChoiceEventInfo) },
        { GameEventType.Cutscene,       info => CreateFromInfo<CutsceneEvent, CutsceneEventInfo>(info as CutsceneEventInfo)},
        { GameEventType.DataLoad,       info => CreateFromInfo<DataLoadEvent, DataLoadEventInfo>(info as DataLoadEventInfo) },
        { GameEventType.DataSave,       info => CreateFromInfo<DataSaveEvent, DataSaveEventInfo>(info as DataSaveEventInfo) },
        { GameEventType.SceneActivate,  info => CreateFromInfo<SceneActivateEvent, SceneActivateEventInfo >(info as SceneActivateEventInfo) },
        { GameEventType.SceneLoad,      info => CreateFromInfo<SceneLoadEvent, SceneLoadEventInfo>(info as SceneLoadEventInfo) },
        { GameEventType.ScreenEffect,   info => CreateFromInfo<ScreenEffectEvent, ScreenEffectEventInfo>(info as ScreenEffectEventInfo) },
        { GameEventType.Sequential,     info => CreateFromInfo<SequentialEvent, SequentialEventInfo>(info as SequentialEventInfo) },   
        { GameEventType.Story,          info => CreateFromInfo<StoryEvent, StoryEventInfo>(info as StoryEventInfo) },
        { GameEventType.UIChange,       info => CreateFromInfo<UIChangeEvent, UIChangeEventInfo>(info as UIChangeEventInfo) },
    };

    private static readonly Dictionary<GameEventType, Func<GameEventDTO, IGameEvent>> _eventCreatorsFromDTO =
        new Dictionary<GameEventType, Func<GameEventDTO, IGameEvent>>
    {
        { GameEventType.Choice,         dto => CreateChoiceEvent(dto as ChoiceEventDTO) },
        { GameEventType.Cutscene,       dto => CreateCutsceneEvent(dto as CutsceneEventDTO) },
        { GameEventType.DataLoad,       dto => CreateDataLoadEvent(dto as DataLoadEventDTO) },
        { GameEventType.DataSave,       dto => CreateDataSaveEvent(dto as DataSaveEventDTO) },
        { GameEventType.SceneActivate,  dto => CreateSceneActivateEvent(dto as SceneActivateEventDTO) },
        { GameEventType.SceneLoad,      dto => CreateSceneLoadEvent(dto as SceneLoadEventDTO) },
        { GameEventType.ScreenEffect,   dto => CreateScreenEffectEvent(dto as ScreenEffectEventDTO) },
        { GameEventType.Sequential,     dto => CreateSequentialEvent(dto as SequentialEventDTO) },   
        { GameEventType.Story,          dto => CreateStoryEvent(dto as StoryEventDTO) },
        { GameEventType.UIChange,       dto => CreateUIChangeEvent(dto as UIChangeEventDTO) },
    };

    private static IGameEvent CreateFromInfo<TEvent, TInfo>(TInfo info)
        where TEvent : GameEventBase<TInfo>
        where TInfo : GameEventInfo
    {
        Assert.IsTrue(null != info, "Cannot create Event from null info");

        var gameEvent = GameEventPool<TEvent, TInfo>.Get(EventHost, info.EventType.ToString());
        gameEvent.Initialize(info);

        return gameEvent;
    }
}
