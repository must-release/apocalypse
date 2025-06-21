using Newtonsoft.Json;


[JsonObject(MemberSerialization.OptOut)]
public class SceneLoadEventDTO : GameEventDTO
{
    public SceneType LoadingScene;
}
