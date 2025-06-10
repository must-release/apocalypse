using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;
using UnityEngine.AddressableAssets;


public class DataSaveEvent : GameEventBase<DataSaveEventInfo>
{
    /****** Public Members ******/

    public override bool ShouldBeSaved => false;
    public override GameEventType EventType => GameEventType.DataSave;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        foreach (GameEventType eventType in activeEventTypeCounts.Keys)
        {
            if (GameEventType.DataSave == eventType || GameEventType.DataLoad == eventType)
                return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not set.");

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(false == DataManager.Instance.IsSaving, "Should not be terminated when data saving is on progress.");
        Assert.IsTrue(null != Info, "Event info is not set before termination");

        if (null != _eventCoroutine)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        Info.DestroyInfo();
        Info = null;

        GameEventPool<DataSaveEvent, DataSaveEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    Coroutine _eventCoroutine;

    private IEnumerator PlayEventCoroutine()
    {
        UIController.Instance.TurnSubUIOn(SubUI.Saving);

        var eventList = GameEventManager.Instance.GetSavableEventInfoList();
        bool shouldTakeScreenShot = true; 
        if (false == GameEventManager.Instance.IsEventTypeActive(GameEventType.Story))
        {
            shouldTakeScreenShot = false;
            eventList.Add(GameEventFactory.CreateUIChangeEvent(BaseUI.Control).EventInfo);
        }
        var dtoList = eventList.ConvertAll(eventInfo => eventInfo.ToDTO());
        DataManager.Instance.SaveUserData(dtoList, Info.SlotNum, shouldTakeScreenShot);

        yield return new WaitWhile(() => DataManager.Instance.IsSaving);

        UIController.Instance.TurnSubUIOff(SubUI.Saving);
        TerminateEvent();
    }
}