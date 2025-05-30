using System.Collections.Generic;
using UnityEngine.Assertions;

/*
 * Load Game Data
 */

public class DataLoadEvent : GameEventBase<DataLoadEventInfo>
{
    /****** Public Members ******/

    public override bool ShouldBeSaved      => false;
    public override bool IsExclusiveEvent   => true;
    public override GameEventType EventType => GameEventType.DataLoad;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        foreach (GameEventType eventType in activeEventTypeCounts.Keys)
        {
            if (GameEventType.Story == eventType || GameEventType.Choice == eventType || GameEventType.Sequential == eventType)
                continue;

            return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not initialized");

        base.PlayEvent();

        if (Info.IsNewGame)
        {
            DataManager.Instance.CreateNewGameData();
        }
        else
        {
            List<GameEventDTO> dtoList = Info.IsContinueGame ?
                DataManager.Instance.LoadRecentData() :
                DataManager.Instance.LoadGameData(Info.SlotNum);

            if (null == dtoList)
            {
                var uiEvent = GameEventFactory.CreateUIChangeEvent(BaseUI.Control);

                if (null != EventContainer)
                {
                    EventContainer.AddNewEvent(uiEvent);
                }
                else
                {
                    GameEventManager.Instance.Submit(uiEvent);
                }
            }
            else
            {
                if (null == EventContainer)
                {
                    foreach (GameEventDTO eventDTO in dtoList)
                    {
                        var gameEvent = GameEventFactory.CreateFromDTO(eventDTO);
                        GameEventManager.Instance.Submit(gameEvent);
                    }
                }
                else
                {
                    EventContainer.OnTerminate += () =>
                    {
                        foreach (GameEventDTO eventDTO in dtoList)
                        {
                            var gameEvent = GameEventFactory.CreateFromDTO(eventDTO);
                            GameEventManager.Instance.Submit(gameEvent);
                        }
                    };
                }
            }
        }

        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not set before termination");


        Info.DestroyInfo();
        Info = null;

        GameEventPool<DataLoadEvent, DataLoadEventInfo>.Release(this);

        base.TerminateEvent();
    }
}