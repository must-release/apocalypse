using Newtonsoft.Json;
using System.Collections.Generic;


[JsonObject(MemberSerialization.OptOut)]
public class ChoiceEventDTO : GameEventDTO
{
    public List<string> ChoiceList;
}
