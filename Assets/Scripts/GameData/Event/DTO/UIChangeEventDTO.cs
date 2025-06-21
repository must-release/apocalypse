using Newtonsoft.Json;


[JsonObject(MemberSerialization.OptOut)]
public class UIChangeEventDTO : GameEventDTO
{
    public BaseUI TargetUI;
}
