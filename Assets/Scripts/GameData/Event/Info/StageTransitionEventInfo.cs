using UnityEngine;
using UnityEngine.Assertions;
using System;

[Serializable]
[CreateAssetMenu(fileName = "NewStageTransitionEventInfo", menuName = "EventInfo/StageTransitionEvent", order = 0)]
public class StageTransitionEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public ChapterType  TargetChapter   { get { return _targetChapter; } private set { _targetChapter = value; } }
    public int          TargetStage     { get { return _targetStage; } private set { _targetStage = value; } }

    public void Initialize(ChapterType targetChapter, int targetStage)
    {
        Assert.IsTrue(ChapterStageCount.IsStageIndexValid(targetChapter, targetStage), $"Invalid target stage: {targetChapter}_{targetStage}");

        _targetChapter = targetChapter;
        _targetStage = targetStage;
        IsInitialized = true;
        IsRuntimeInstance = true;
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
        EventType = GameEventType.StageTransition;
    }

    protected override void OnValidate()
    {
        if (_targetChapter != ChapterType.ChapterTypeCount && _targetStage > 0)
        {
            Assert.IsTrue(ChapterStageCount.IsStageIndexValid(_targetChapter, _targetStage), $"Invalid target stage: {_targetChapter}_{_targetStage}");
        }
    }


    /****** Private Members ******/

    [SerializeField] private ChapterType    _targetChapter  = ChapterType.ChapterTypeCount;
    [SerializeField] private int            _targetStage    = 0;
}