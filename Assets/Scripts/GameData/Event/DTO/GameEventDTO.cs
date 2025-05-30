using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptOut)]
public class GameEventDTO
{
    public GameEventType EventType;
}
