using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using AD.UI;
using AD.Story;

public class SideDialogueEvent : GameEventBase<SideDialogueEventInfo>
{
    /****** Public Members ******/

    public override bool ShouldBeSaved      => false;
    public override bool IsExclusiveEvent   => false;
    public override GameEventType EventType => GameEventType.SideDialogue;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        if (activeEventTypeCounts.ContainsKey(GameEventType.SideDialogue))
            return false;

        return true;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        base.PlayEvent();
        PlayEventAsync().Forget();
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");
        Debug.Assert(null != _cancellationTokenSource, "CancellationTokenSource is null in side dialogue event");

        _cancellationTokenSource.Cancel();

        Info.DestroyInfo();
        Info = null;

        GameEventPool<SideDialogueEvent, SideDialogueEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private CancellationTokenSource _cancellationTokenSource;

    private async UniTask PlayEventAsync()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        _cancellationTokenSource = new CancellationTokenSource();

        ControlUIView controlUI = UIController.Instance.GetUIView(BaseUI.Control) as ControlUIView;
        Debug.Assert(null != controlUI, "Cannot find control UI in side dialogue event.");

        OpResult result = await controlUI.ShowSideDialogue(Info.Text, Info.TextInterval, _cancellationTokenSource.Token);
        Debug.Assert(OpResult.Success == result, $"Side dialogue event not Successful. status: {result}");
        
        TerminateEvent();
    }
}