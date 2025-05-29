using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


[JsonObject(MemberSerialization.OptOut)]
public class DataLoadEventDTO : GameEventDTO
{
    public int SlotNum;
    public bool IsNewGame;
    public bool IsContinueGame;
}
