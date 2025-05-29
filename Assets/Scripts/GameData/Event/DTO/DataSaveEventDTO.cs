using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


[JsonObject(MemberSerialization.OptOut)]
public class DataSaveEventDTO : GameEventDTO
{
    public int SlotNum;
}
