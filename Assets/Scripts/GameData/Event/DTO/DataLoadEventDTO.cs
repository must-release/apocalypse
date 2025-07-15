using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


[JsonObject(MemberSerialization.Fields)]
public class DataLoadEventDTO : GameEventDTO
{
    public DataSlotType SlotType            { get { return _slotType; } set { _slotType = value; } }
    public ChapterType  LoadingChapter      { get { return _loadingChapter; } set { _loadingChapter = value; } }
    public int          LoadingStage        { get { return _loadingStage; } set { _loadingStage = value; } }
    public bool         IsContinueGame      { get { return _isContinueGame; } set { _isContinueGame = value; } }
    public bool         IsCreatingNewData   { get { return _isCreatingNewData; } set { _isCreatingNewData = value; } }

    private DataSlotType    _slotType;
    private ChapterType     _loadingChapter;
    private int             _loadingStage;
    private bool            _isContinueGame;
    private bool            _isCreatingNewData;
}
