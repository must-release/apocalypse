using Newtonsoft.Json;


[JsonObject(MemberSerialization.OptOut)]
public class ScreenEffectEventDTO : GameEventDTO
{
    public ScreenEffect ScreenEffectType;
}
