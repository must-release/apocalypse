using Newtonsoft.Json;
using SceneEnums;


[JsonObject(MemberSerialization.OptOut)]
public class SceneLoadEventDTO : GameEventDTO
{
    public SceneName LoadingScene;
}
