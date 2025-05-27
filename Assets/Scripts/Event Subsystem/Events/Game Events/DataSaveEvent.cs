using UnityEngine;
using UIEnums;
using EventEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;
using UnityEngine.AddressableAssets;


public class DataSaveEvent : GameEventBase<DataSaveEventInfo>
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => false;
    public override GameEventInfo   EventInfo       => _info;
    public override GameEventType   EventType       => GameEventType.DataSave;

    public override void Initialize(DataSaveEventInfo eventInfo, IGameEvent parentEvent = null)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info       = eventInfo;
        Status      = EventStatus.Waiting;
        ParentEvent = parentEvent;
    }

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        foreach (GameEventType eventType in activeEventTypeCounts.Keys)
        {
            if (GameEventType.DataSave == eventType || GameEventType.DataLoad == eventType)
                return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue( null != _info, "Event info is not set." );

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(false == DataManager.Instance.IsSaving, "Should not be terminated when data saving is on progress.");
        Assert.IsTrue(null != _info, "Event info is not set before termination");

        if (null != _eventCoroutine)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        _info.DestroyInfo();
        _info = null;

        GameEventPool<DataSaveEvent, DataSaveEventInfo>.Release(this);

        base.TerminateEvent();
    }
    

    /****** Private Members ******/

    DataSaveEventInfo   _info               = null;
    Coroutine           _eventCoroutine     = null;

    // TODO : Move GetStartingEvent and GetRootEvent function to GameEventManager
    private IEnumerator PlayEventCoroutine()
    {
        UIController.Instance.TurnSubUIOn(SubUI.Saving);

        bool shouldTakeScreenShot = false;
        if (GameEventManager.Instance.IsEventTypeActive(GameEventType.Story))
        {
            var storyInfo = StoryController.Instance.GetStoryProgressInfo();
            (GameEventManager.Instance.GetActiveEvent(GameEventType.Story) as StoryEvent).UpdateStoryProgress(storyInfo);
            shouldTakeScreenShot = true;
        }


        // // Save user data
        // GameEvent startingEvent = GetStartingEvent();
        // DataManager.Instance.SaveUserData(startingEvent, _info.SlotNum, shouldTakeScreenShot);

        yield return new WaitWhile(() => DataManager.Instance.IsSaving );

        UIController.Instance.TurnSubUIOff(SubUI.Saving);
        TerminateEvent();
    }

    // private GameEvent GetStartingEvent()
    // {
    //     GameEvent startingEvent = GetRootEvent();
    //     if(startingEvent == null)
    //     {
    //         return NextEvent;
    //     }
    //     else
    //     {
    //         return startingEvent;
    //     }
    // }

    // private GameEvent GetRootEvent()
    // {            
    //     if(ParentEvent == null)
    //     {
    //         return null;
    //     }
    //     else
    //     {
    //         GameEvent root = ParentEvent;
    //         while(root.ParentEvent != null)
    //         {
    //             root = root.ParentEvent;
    //         }
    //         return root;
    //     }
    // }
}