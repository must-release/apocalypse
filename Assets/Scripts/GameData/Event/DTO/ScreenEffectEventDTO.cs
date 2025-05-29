using Newtonsoft.Json;
using ScreenEffectEnums;


[JsonObject(MemberSerialization.OptOut)]
public class ScreenEffectEventDTO : GameEventDTO
{
    public ScreenEffect ScreenEffectType;
}
