using UnityEngine;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewDataLoadEventInfo", menuName = "EventInfo/DataLoadEvent", order = 0)]
public class DataLoadEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public DataSlotType SlotType                { get { return _slotType; } private set { _slotType = value; }}
    public ChapterType  LoadingChapter          { get { return _loadingChapter; } private set { _loadingChapter = value; } }
    public int          LoadingStage            { get { return _loadingStage; } private set { _loadingStage = value; } }
    public bool         IsContinueGame          { get { return _isContinueGame; } private set { _isContinueGame = value; }}
    public bool         IsCreatingNewData       { get { return _isCreatingNewData; } private set { _isCreatingNewData = value; } }

    public void Initialize() // Default initialization is continue game
    {
        IsContinueGame      = true;
        IsInitialized       = true;
        IsRuntimeInstance   = true;
    }

    public void Initialize(DataSlotType slotType)
    {
        Assert.IsTrue(slotType != DataSlotType.DataSlotTypeCount, "Invalid data slot type for DataLoadEventInfo initialization.");

        _slotType               = slotType;
        IsInitialized           = true;
        IsRuntimeInstance       = true;
    }

    public void Initialize(ChapterType loadingChapter, int loadingStage)
    {
        _isCreatingNewData  = true;
        _loadingChapter     = loadingChapter;
        _loadingStage       = loadingStage;
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
        Assert.IsTrue(_isCreatingNewData ^ _isContinueGame, "Both continue game and loading from slot cannot be executed.");
    }

    public override GameEventDTO ToDTO()
    {
        return new DataLoadEventDTO
        {
            EventType           = EventType,
            SlotType            = _slotType,
            LoadingChapter      = _loadingChapter,
            LoadingStage        = _loadingStage,
            IsContinueGame      = _isContinueGame,
            IsCreatingNewData   = _isCreatingNewData
        };
    }

    /****** Private Members ******/

    [SerializeField] private DataSlotType   _slotType           = DataSlotType.DataSlotTypeCount;
    [SerializeField] private ChapterType    _loadingChapter     = ChapterType.ChapterTypeCount;
    [SerializeField] private int            _loadingStage       = 0;
    [SerializeField] private bool           _isContinueGame     = false; // If true, load most recent saved data
    [SerializeField] private bool           _isCreatingNewData  = false; // If true, load from a specific data slot, otherwise create new data
}
