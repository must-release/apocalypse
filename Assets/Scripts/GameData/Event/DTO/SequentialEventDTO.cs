using Newtonsoft.Json;
using ScreenEffectEnums;
using System.Collections.Generic;


[JsonObject(MemberSerialization.OptOut)]
public class SequentialEventDTO : GameEventDTO
{
    public List<GameEventDTO> EventDTOs;
    public int StartIndex;
}
