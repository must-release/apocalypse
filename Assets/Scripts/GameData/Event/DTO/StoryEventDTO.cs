using Newtonsoft.Json;


[JsonObject(MemberSerialization.OptOut)]
public class StoryEventDTO : GameEventDTO
{
    public ChapterType StoryStage;
    public int StoryNumber;
    public int ReadBlockCount;
    public int ReadEntryCount;
    public bool IsOnMap;
}
