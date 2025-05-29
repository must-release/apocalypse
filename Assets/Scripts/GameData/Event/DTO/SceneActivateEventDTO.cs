using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


[JsonObject(MemberSerialization.OptOut)]
public class SceneActivateEventDTO : GameEventDTO
{
    public bool ShouldTurnOnLoadingUI;
}
