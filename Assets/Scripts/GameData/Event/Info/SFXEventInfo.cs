using UnityEngine;

[CreateAssetMenu(fileName = "SFXEventInfo", menuName = "GameData/Event/SFXEventInfo")]
public class SFXEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public string ClipName => _clipName;

    public void Initialize(string clipName)
    {
        _clipName       = clipName;

        IsInitialized   = true;
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


    /****** Private Members ******/

    [SerializeField] private string _clipName;
}