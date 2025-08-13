using UnityEngine;

[CreateAssetMenu(fileName = "BGMEventInfo", menuName = "GameData/Event/BGMEventInfo")]
public class BGMEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public bool ShouldStop;
    public string ClipName;

    public void Initialize(bool shouldStop, string clipName = "")
    {
        ShouldStop = shouldStop;
        ClipName = clipName;

        IsInitialized = true;
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
        EventType = GameEventType.BGM;
    }

    protected override void OnValidate() { }
}