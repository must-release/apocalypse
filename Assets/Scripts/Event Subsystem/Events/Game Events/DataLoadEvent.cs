﻿using UnityEngine;
using System.Collections.Generic;
using UIEnums;
using EventEnums;
using UnityEngine.Assertions;

/*
 * Load Game Data
 */

public class DataLoadEvent : GameEvent
{
    /****** Public Members ******/

    public void SetEventInfo(DataLoadEventInfo eventInfo)
    {
        Assert.IsTrue(eventInfo.IsInitialized, "Event info is not initialized");

        _dataLoadEventInfo = eventInfo;
    }

    public override bool CheckCompatibility(GameEvent parentEvent, Stack<GameEvent> pararrelEvents)
    {
        // Can be played when current base UI is title or load
        if (parentEvent == null || parentEvent.EventType == GameEventType.Story || parentEvent.EventType == GameEventType.Choice)
            return true;
        
        return false;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue( null != _dataLoadEventInfo, "Event info is not initialized" );


        if (_dataLoadEventInfo.IsNewGame) // Create new game data
        {
            DataManager.Instance.CreateNewGameData();
        }
        else // Load game data
        {
            // Load data and get starting event
            GameEvent startingEvent = _dataLoadEventInfo.IsContinueGame ? 
                DataManager.Instance.LoadRecentData():
                DataManager.Instance.LoadGameData(_dataLoadEventInfo.SlotNum);

            // Apply starting event
            if( null == startingEvent) // When there is no starting event, concat UI change event to current event chain
            {
                // TODO : GameEventList에 추가하는 방식으로 변경
                // UIChangeEvent uiEvent = CreateInstance<UIChangeEvent>();
                // uiEvent.changingUI = BaseUI.Control;
                // ConcatEvent(uiEvent);
            }
            else // When there is starting event, concat it to current event chain
            {
                ConcatEvent(startingEvent);
            }
        }

        GameEventManager.Instance.TerminateGameEvent(this);
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _dataLoadEventInfo, "Event info is not set before termination");

        ScriptableObject.Destroy(_dataLoadEventInfo);
        _dataLoadEventInfo = null;

        GameEventPool<DataLoadEvent>.Release(this);
    }

    public override GameEventInfo GetEventInfo()
    {
        return _dataLoadEventInfo;
    }

    /****** Private Members ******/

    private DataLoadEventInfo _dataLoadEventInfo = null;

    // Concat starting event to current event chain
    private void ConcatEvent(GameEvent startingEvent)
    {
        GameEvent lastEvent = this;

        while (lastEvent.NextEvent != null)
        {
            lastEvent = lastEvent.NextEvent;
        }

        //lastEvent.NextEvent = startingEvent;
    }
}


[CreateAssetMenu(fileName = "NewDataLoadEvent", menuName = "EventInfo/DataLoadEvent", order = 0)]
public class DataLoadEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public int SlotNum { get { return _slotNum; } private set { _slotNum = value; }}
    public bool IsNewGame { get {return _isNewGame; } private set { _isNewGame = value; }}
    public bool IsContinueGame { get { return _isContinueGame; } private set { _isContinueGame = value; }}
    
    public void Initialize(int slotNum, bool isNewGame, bool isContinueGame)
    {
        Assert.IsTrue( false == IsInitialized,          "Duplicate initialization of GameEventInfo is not allowed." );
        Assert.IsFalse( isNewGame && isContinueGame,    "It can't be both New Game and Continue Game." );

        SlotNum         = slotNum;
        IsNewGame       = isNewGame;
        IsContinueGame  = isContinueGame;
        IsInitialized   = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.DataLoad;
    }

    protected override void OnValidate()
    {
        if ( _isNewGame && _isContinueGame )
            Debug.LogError("New Game 이면서 Continue Game일 수는 없습니다.");

        if ( 0 < _slotNum )
            IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private int    _slotNum        = -1; // Number of the data slot to load data
    [SerializeField] private bool   _isNewGame      = false; // If true, create new game data
    [SerializeField] private bool   _isContinueGame = false; // If true, load most recent saved data
}
