using UnityEngine;
using System.Collections;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewDataLoad", menuName = "Event/DataLoadEvent", order = 0)]
public class DataLoadEvent : GameEvent
{
    public int slotNum; // Number of the data slot to load data
    public bool isNewGame = false; // If true, create new game data
    public bool isContinueGame = false; // If true, load most recent saved data

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.DATA_LOAD;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, (BASEUI, SUBUI) currentUI)
    {
        // Can be played when current base UI is title or load
        if (currentUI.Item1 == BASEUI.TITLE || currentUI.Item2 == SUBUI.LOAD)
        {
            return true;
        }
        else
            return false;
    }

    // Play Data Load Event
    public override void PlayEvent()
    {
        if (isNewGame) // Create new game data
        {
            DataManager.Instance.CreateNewGameData();
        }
        else // Load game data
        {
            if (isContinueGame) DataManager.Instance.LoadRecentData();
            else DataManager.Instance.LoadGameData(slotNum);

            // Get starting event
            GameEvent startingEvent = PlayerManager.Instance.PlayerData.StartingEvent;

            // Apply starting event
            if(startingEvent == null) // When there is no starting event, concat UI change event to current event chain
            {
                UIChangeEvent uiEvent = CreateInstance<UIChangeEvent>();
                uiEvent.changingUI = BASEUI.CONTROL;
                ConcatEvent(uiEvent);
            }
            else // When there is starting event, concat it to current event chain
            {
                ConcatEvent(startingEvent);
            }
        }

        // Terminate data load event and play next event
        GameEventManager.Instance.TerminateGameEvent(this);
    }

    // Concat starting event to current event chain
    public void ConcatEvent(GameEvent startingEvent)
    {
        GameEvent lastEvent = this;

        while (lastEvent.nextEvent != null)
        {
            lastEvent = lastEvent.nextEvent;
        }

        lastEvent.NextEvent = startingEvent;
    }
}

