using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class GameEventFactory
{
    /****** Public Members ******/

    public static IEnumerator LoadCommonGameEvents()
    {
        Debug.Assert(0 == _commonEventInfos.Count, "Duplicate Common event load.");

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
        Debug.Assert(null != dto, "Cannot create DataLoadEvent from null DTO");
        return CreateCutsceneEvent();
    }

    public static IGameEvent CreateDataLoadEvent()
    {
        var info = ScriptableObject.CreateInstance<DataLoadEventInfo>();
        info.Initialize();

        var evt = GameEventPool<DataLoadEvent, DataLoadEventInfo>.Get(EventHost, "DataLoadEvent");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateDataLoadEvent(DataSlotType slotType)
    {
        var info = ScriptableObject.CreateInstance<DataLoadEventInfo>();
        info.Initialize(slotType);

        var evt = GameEventPool<DataLoadEvent, DataLoadEventInfo>.Get(EventHost, $"DataLoadEvent_{slotType}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateDataLoadEvent(ChapterType loadingChapter, int loadingStage)
    {
        var info = ScriptableObject.CreateInstance<DataLoadEventInfo>();
        info.Initialize(loadingChapter, loadingStage);
        
        var evt = GameEventPool<DataLoadEvent, DataLoadEventInfo>.Get(EventHost, $"DataLoadEvent_{loadingChapter}_{loadingStage}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateDataLoadEvent(DataLoadEventDTO dto)
    {
        Debug.Assert(null != dto, "Cannot create DataSaveEvent from null DTO");

        if (dto.IsContinueGame)
        {
            return CreateDataLoadEvent();
        }
        else if (dto.IsCreatingNewData)
        {
            return CreateDataLoadEvent(dto.LoadingChapter, dto.LoadingStage);
        }
        else
        {
            return CreateDataLoadEvent(dto.SlotType);
        }
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
        Debug.Assert(null != dto, "Cannot create DataSaveEvent from null DTO");
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
        Debug.Assert(null != dto, "Cannot create SceneActivateEvent from null DTO");
        return CreateSceneActivateEvent(dto.ShouldTurnOnLoadingUI);
    }

    public static IGameEvent CreateSceneLoadEvent(SceneType loadingScene)
    {
        var info = ScriptableObject.CreateInstance<SceneLoadEventInfo>();
        info.Initialize(loadingScene);

        var evt = GameEventPool<SceneLoadEvent, SceneLoadEventInfo>.Get(EventHost, $"SceneLoadEvent_{loadingScene}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateSceneLoadEvent(SceneLoadEventDTO dto)
    {
        Debug.Assert(null != dto, "Cannot create SceneLoadEvent from null DTO");
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

    public static IGameEvent CreateStoryEvent(ChapterType storyStage, int storyNumber, int readBlockCount, int readEntryCount, bool isOnMap)
    {
        var info = ScriptableObject.CreateInstance<StoryEventInfo>();
        info.Initialize(storyStage, storyNumber, readBlockCount, readEntryCount, isOnMap);

        var evt = GameEventPool<StoryEvent, StoryEventInfo>.Get(EventHost, $"StoryEvent_{storyStage}_{storyNumber}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateStoryEvent(StoryEventDTO dto)
    {
        Debug.Assert(null != dto, "Cannot create StoryEvent from null DTO");
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
        Debug.Assert(null != dto, "Cannot create UIChangeEvent from null DTO");
        return CreateUIChangeEvent(dto.TargetUI);
    }

    public static IGameEvent CreateAudioEvent(bool isBgm, AudioAction action, string clipName = "", float volume = 1.0f)
    {
        var info = ScriptableObject.CreateInstance<AudioEventInfo>();
        info.Initialize(isBgm, action, clipName, volume);

        var evt = GameEventPool<AudioEvent, AudioEventInfo>.Get(EventHost, $"AudioEvent_{action}_{clipName}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateSequentialEvent(List<IGameEvent> gameEvents, int startIndex = 0)
    {
        Debug.Assert(null != gameEvents && 0 < gameEvents.Count, "GameEvents list is null or empty");

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
        Debug.Assert(null != dto, "Cannot create SequentialEvent from null DTO");

        var eventList = new List<IGameEvent>();

        foreach (var eventDTO in dto.EventDTOs)
        {
            eventList.Add(CreateFromDTO(eventDTO));
        }

        return CreateSequentialEvent(eventList, dto.StartIndex);
    }

    public static IGameEvent CreateStageTransitionEvent(ChapterType targetChapter, int targetStage)
    {
        var info = ScriptableObject.CreateInstance<StageTransitionEventInfo>();
        info.Initialize(targetChapter, targetStage);

        var evt = GameEventPool<StageTransitionEvent, StageTransitionEventInfo>.Get(EventHost, $"StageTransitionEvent_{targetChapter}_{targetStage}");
        evt.Initialize(info);
        return evt;
    }

    public static IGameEvent CreateFallDeathEvent(int hpDamage = 10)
    {
        var info = ScriptableObject.CreateInstance<FallDeathEventInfo>();
        info.Initialize(hpDamage);

        var evt = GameEventPool<FallDeathEvent, FallDeathEventInfo>.Get(EventHost, $"FallDeathEvent_{hpDamage}");
        evt.Initialize(info);
        return evt;
    }

    

    public static IGameEvent CreateFromInfo<TEventInfo>(TEventInfo info)
        where TEventInfo : GameEventInfo
    {
        Debug.Assert(null != info, "Cannot create event from null info");
        Debug.Assert(true == _eventCreatorsFromInfo.ContainsKey(info.EventType), $"No info creator registered for GameEventType: {info.EventType}");

        return _eventCreatorsFromInfo[info.EventType].Invoke(info);
    }

    public static IGameEvent CreateFromDTO(GameEventDTO dto)
    {
        Debug.Assert(null != dto, "Cannot create event from null DTO");
        Debug.Assert(true == _eventCreatorsFromDTO.ContainsKey(dto.EventType), $"No dto creator registered for GameEventType: {dto.EventType}");

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
        { GameEventType.Cutscene,           info => CreateFromInfo<CutsceneEvent, CutsceneEventInfo>(info as CutsceneEventInfo)},
        { GameEventType.DataLoad,           info => CreateFromInfo<DataLoadEvent, DataLoadEventInfo>(info as DataLoadEventInfo) },
        { GameEventType.DataSave,           info => CreateFromInfo<DataSaveEvent, DataSaveEventInfo>(info as DataSaveEventInfo) },
        { GameEventType.SceneActivate,      info => CreateFromInfo<SceneActivateEvent, SceneActivateEventInfo >(info as SceneActivateEventInfo) },
        { GameEventType.SceneLoad,          info => CreateFromInfo<SceneLoadEvent, SceneLoadEventInfo>(info as SceneLoadEventInfo) },
        { GameEventType.ScreenEffect,       info => CreateFromInfo<ScreenEffectEvent, ScreenEffectEventInfo>(info as ScreenEffectEventInfo) },
        { GameEventType.Sequential,         info => CreateFromInfo<SequentialEvent, SequentialEventInfo>(info as SequentialEventInfo) },   
        { GameEventType.StageTransition,    info => CreateFromInfo<StageTransitionEvent, StageTransitionEventInfo>(info as StageTransitionEventInfo) },
        { GameEventType.Story,              info => CreateFromInfo<StoryEvent, StoryEventInfo>(info as StoryEventInfo) },
        { GameEventType.UIChange,           info => CreateFromInfo<UIChangeEvent, UIChangeEventInfo>(info as UIChangeEventInfo) },
        { GameEventType.Audio,              info => CreateFromInfo<AudioEvent, AudioEventInfo>(info as AudioEventInfo) },
    };

    private static readonly Dictionary<GameEventType, Func<GameEventDTO, IGameEvent>> _eventCreatorsFromDTO =
        new Dictionary<GameEventType, Func<GameEventDTO, IGameEvent>>
    {
        { GameEventType.Cutscene,           dto => CreateCutsceneEvent(dto as CutsceneEventDTO) },
        { GameEventType.DataLoad,           dto => CreateDataLoadEvent(dto as DataLoadEventDTO) },
        { GameEventType.DataSave,           dto => CreateDataSaveEvent(dto as DataSaveEventDTO) },
        { GameEventType.SceneActivate,      dto => CreateSceneActivateEvent(dto as SceneActivateEventDTO) },
        { GameEventType.SceneLoad,          dto => CreateSceneLoadEvent(dto as SceneLoadEventDTO) },
        { GameEventType.Sequential,         dto => CreateSequentialEvent(dto as SequentialEventDTO) },   
        { GameEventType.Story,              dto => CreateStoryEvent(dto as StoryEventDTO) },
        { GameEventType.UIChange,           dto => CreateUIChangeEvent(dto as UIChangeEventDTO) },
    };

    private static IGameEvent CreateFromInfo<TEvent, TInfo>(TInfo info)
        where TEvent : GameEventBase<TInfo>
        where TInfo : GameEventInfo
    {
        Debug.Assert(null != info, "Cannot create Event from null info");

        var gameEvent = GameEventPool<TEvent, TInfo>.Get(EventHost, info.EventType.ToString());
        gameEvent.Initialize(info);

        return gameEvent;
    }
}
