using UnityEngine;
using System.Collections.Generic;
using AD.Audio;

public class AudioEvent : GameEventBase<AudioEventInfo>
{
    public override bool ShouldBeSaved      => false;
    public override GameEventType EventType => GameEventType.Audio;
    public override bool IsExclusiveEvent   => false;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        return true;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        base.PlayEvent();

        if (true == Info.IsBGM)
        {
            switch (Info.Action)
            {
                case AudioAction.Play:
                    AudioManager.Instance.PlayBGM(Info.ClipName);
                    break;
                case AudioAction.Stop:
                    AudioManager.Instance.StopBGM();
                    break;
                case AudioAction.SetVolume:
                    AudioManager.Instance.SetBGMVolume(Info.Volume);
                    break;
            }
        }
        else
        {
            switch (Info.Action)
            {
                case AudioAction.Play:
                    AudioManager.Instance.PlaySFX(Info.ClipName);
                    break;
                case AudioAction.SetVolume:
                    AudioManager.Instance.SetSFXVolume(Info.Volume);
                    break;
            }
        }

        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");

        Info.DestroyInfo();
        Info = null;

        GameEventPool<AudioEvent, AudioEventInfo>.Release(this);

        base.TerminateEvent();
    }
}