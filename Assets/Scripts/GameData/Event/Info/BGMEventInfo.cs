using UnityEngine;

[CreateAssetMenu(fileName = "BGMEventInfo", menuName = "GameData/Event/BGMEventInfo")]
public class BGMEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public bool ShouldStop => _shouldStop;
    public string ClipName => _clipName;

    public void Initialize(bool shouldStop, string clipName = "")
    {
        _shouldStop     = shouldStop;
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
        EventType = GameEventType.BGM;
    }

    protected override void OnValidate() { }


    /****** Private Members ******/

    [SerializeField] private bool _shouldStop;
    [SerializeField] private string _clipName;
}