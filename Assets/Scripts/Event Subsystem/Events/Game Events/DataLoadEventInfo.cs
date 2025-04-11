using UnityEngine;
using EventEnums;
using UnityEngine.Assertions;
using System;


[Serializable]
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
