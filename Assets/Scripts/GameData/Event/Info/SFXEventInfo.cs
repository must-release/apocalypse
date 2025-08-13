using UnityEngine;

[CreateAssetMenu(fileName = "SFXEventInfo", menuName = "GameData/Event/SFXEventInfo")]
public class SFXEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public string ClipName;

    public void Initialize(string clipName)
    {
        ClipName = clipName;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.IsRuntimeInstance = true;

        return clone;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.SFX;
    }

    protected override void OnValidate() { }
}