using Newtonsoft.Json;
using StageEnums;
using UnityEngine;


[JsonObject(MemberSerialization.OptOut)]
public class StoryEventDTO : GameEventDTO
{
    public Stage StoryStage;
    public int StoryNumber;
    public int ReadBlockCount;
    public int ReadEntryCount;
    public bool IsOnMap;
}
