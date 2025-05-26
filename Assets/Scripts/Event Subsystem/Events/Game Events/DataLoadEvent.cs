using UnityEngine;
using System.Collections.Generic;
using EventEnums;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;

/*
 * Load Game Data
 */

public class DataLoadEvent : GameEvent
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => false; 
    public override GameEventInfo   EventInfo       => _info;
    public override GameEventType   EventType       => GameEventType.DataLoad;

    public void SetEventInfo(DataLoadEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid");

        _info   = eventInfo;
        Status  = EventStatus.Waiting;
    }

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        foreach (GameEventType eventType in activeEventTypeCounts.Keys)
        {
            if (GameEventType.Story == eventType || GameEventType.Choice == eventType || GameEventType.Sequential == eventType)
                continue;

            return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue( null != _info, "Event info is not initialized" );


        base.PlayEvent();

        if (_info.IsNewGame) // Create new game data
        {
            DataManager.Instance.CreateNewGameData();
        }
        else // Load game data
        {
            // Load data and get starting event
            List<GameEventInfo> activeEventInfoList = _info.IsContinueGame ? 
                DataManager.Instance.LoadRecentData():
                DataManager.Instance.LoadGameData(_info.SlotNum);

            // Apply starting event
            if( null == activeEventInfoList) // When there is no starting event, concat UI change event to current event chain
            {
                // TODO : GameEventList에 추가하는 방식으로 변경
                // UIChangeEvent uiEvent = CreateInstance<UIChangeEvent>();
                // uiEvent.changingUI = BaseUI.Control;
                // ConcatEvent(uiEvent);
            }
            else // When there is starting event, concat it to current event chain
            {
                //ConcatEvent(startingEvent);
            }
        }

        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not set before termination");

        // Todo: Release the event info
        // if (_info.IsFromAddressables) Addressables.Release(_info);
        // else Destroy(_info);
        _info = null;

        GameEventPool<DataLoadEvent>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private DataLoadEventInfo _info = null;

    // // Concat starting event to current event chain
    // private void ConcatEvent(GameEvent startingEvent)
    // {
    //     GameEvent lastEvent = this;

    //     while (lastEvent.NextEvent != null)
    //     {
    //         lastEvent = lastEvent.NextEvent;
    //     }

    //     //lastEvent.NextEvent = startingEvent;
    // }
}