using Newtonsoft.Json;
using ScreenEffectEnums;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;


[JsonObject(MemberSerialization.OptOut)]
public class UIChangeEventDTO : GameEventDTO
{
    public BaseUI TargetUI;
}
