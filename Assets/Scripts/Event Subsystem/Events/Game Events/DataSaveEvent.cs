using UnityEngine;
using UIEnums;
using EventEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;


public class DataSaveEvent : GameEvent
{
    /****** Public Members ******/

    public override bool ShouldBeSaved() => false;
    
    public void SetEventInfo(DataSaveEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info   = eventInfo;
        Status  = EventStatus.Waiting;
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
        _eventCoroutine = StartCoroutine( PlayEventCoroutine() );
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

        ScriptableObject.Destroy(_info);
        _info = null;

        GameEventPool<DataSaveEvent>.Release(this);

        base.TerminateEvent();
    }

    public override GameEventInfo GetEventInfo() => _info;
    
    public override GameEventType GetEventType() => GameEventType.DataSave;
    

    /****** Private Members ******/

    DataSaveEventInfo   _info  = null;
    Coroutine           _eventCoroutine     = null;

    // TODO : Move GetStartingEvent and GetRootEvent function to GameEventManager
    private IEnumerator PlayEventCoroutine()
    {
        // Turn Saving UI on
        UIController.Instance.TurnSubUIOn(SubUI.Saving);

        // When saving during story mode
        // bool shouldTakeScreenShot = false;
        // GameEvent rootEvent = GetRootEvent();
        // if (rootEvent?.EventType == GameEventType.Story)
        // {
        //     // Get current story progress info
        //     var storyInfo = StoryController.Instance.GetStoryProgressInfo();

        //     // Update StoryEvent
        //     (rootEvent as StoryEvent).UpdateStoryProgress(storyInfo);

        //     // Take screen shot when saving
        //     shouldTakeScreenShot = true;
        // }

        // // Save user data
        // GameEvent startingEvent = GetStartingEvent();
        // DataManager.Instance.SaveUserData(startingEvent, _info.SlotNum, shouldTakeScreenShot);

        // Wait while saving
        yield return new WaitWhile(() => DataManager.Instance.IsSaving );

        // Turn Saving UI off
        UIController.Instance.TurnSubUIOff(SubUI.Saving);

        // Terminate data save event and play next event
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