using Newtonsoft.Json;

[JsonObject(MemberSerialization.Fields)]
public class StageTransitionEventDTO : GameEventDTO
{
    public ChapterType TargetChapter { get { return _targetChapter; } set { _targetChapter = value; } }
    public int TargetStage { get { return _targetStage; } set { _targetStage = value; } }

    private ChapterType _targetChapter;
    private int _targetStage;
}