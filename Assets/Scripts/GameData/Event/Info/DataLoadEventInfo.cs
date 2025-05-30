using UnityEngine;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewDataLoadEventInfo", menuName = "EventInfo/DataLoadEvent", order = 0)]
public class DataLoadEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public int SlotNum { get { return _slotNum; } private set { _slotNum = value; }}
    public bool IsNewGame { get {return _isNewGame; } private set { _isNewGame = value; }}
    public bool IsContinueGame { get { return _isContinueGame; } private set { _isContinueGame = value; }}
    
    public void Initialize(int slotNum, bool isNewGame, bool isContinueGame, bool isRuntimeInstance = false)
    {
        Assert.IsTrue( false == IsInitialized,          "Duplicate initialization of GameEventInfo is not allowed." );
        Assert.IsFalse( isNewGame && isContinueGame,    "It can't be both New Game and Continue Game." );

        SlotNum             = slotNum;
        IsNewGame           = isNewGame;
        IsContinueGame      = isContinueGame;
        IsInitialized       = true;
        IsRuntimeInstance   = true;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.IsRuntimeInstance = true;

        return clone;
    }

    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.DataLoad;
    }

    protected override void OnValidate()
    {
        if (_isNewGame && _isContinueGame)
            Debug.LogError("New Game 이면서 Continue Game 일 수는 없습니다.");

        if (0 < _slotNum || IsNewGame || IsContinueGame)
            IsInitialized = true;
    }
    public override GameEventDTO ToDTO()
    {
        return new DataLoadEventDTO
        {
            EventType       = EventType,
            IsNewGame       = _isNewGame,
            IsContinueGame  = _isContinueGame,
            SlotNum         = _slotNum
        };
    }

    /****** Private Members ******/

    [SerializeField] private int    _slotNum        = -1; // Number of the data slot to load data
    [SerializeField] private bool   _isNewGame      = false; // If true, create new game data
    [SerializeField] private bool   _isContinueGame = false; // If true, load most recent saved data
}
