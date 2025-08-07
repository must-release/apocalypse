using UnityEngine;

public enum AudioAction
{
    Play,
    Stop,
    SetVolume
}

[CreateAssetMenu(fileName = "AudioEventInfo", menuName = "GameData/Event/AudioEventInfo")]
public class AudioEventInfo : GameEventInfo
{ 
    public bool IsBGM;
    public AudioAction Action;
    public string ClipName;
    public float Volume;

    public override GameEventInfo Clone()
    {
        AudioEventInfo newInfo = Instantiate(this);
        newInfo.IsRuntimeInstance = true;
        return newInfo;
    }

    protected override void OnEnable()
    {
        if (false == IsInitialized)
        {
            EventType       = GameEventType.Audio;
            IsInitialized   = true;
        }
    }

    protected override void OnValidate() { }

    public void Initialize(bool isBgm, AudioAction action, string clipName = "", float volume = 1.0f)
    {
        IsBGM = isBgm;
        Action = action;
        ClipName = clipName;
        Volume = volume;
    }
}
