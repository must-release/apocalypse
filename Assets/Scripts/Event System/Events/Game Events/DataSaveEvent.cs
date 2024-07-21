using UnityEngine;
using UIEnums;
using EventEnums;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "NewDataSave", menuName = "Event/DataSaveEvent", order = 0)]
public class DataSaveEvent : GameEvent
{
    public int slotNum; // if 0 auto save, else save in slot

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.DATA_SAVE;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, (BASEUI, SUBUI) currentUI)
    {
        // Can be played when current base UI is loading or save
        if (currentUI.Item1 == BASEUI.LOADING || currentUI.Item2 == SUBUI.SAVE)
        {
            return true;
        }
        else if(parentEvent == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Play Data Save Event
    public override void PlayEvent()
    {
        // Use GameEventManger to start coroutine
        GameEventManager.Instance.StartCoroutineForGameEvents(PlayEventCoroutine());
    }
    IEnumerator PlayEventCoroutine()
    {
        // Turn Saving UI on
        UIController.Instance.TurnSubUIOn(SUBUI.SAVING);

        if (ParentEvent && ParentEvent.EventType == EVENT_TYPE.STORY)
        {
            // Get current story progress info
            var storyInfo = StoryController.Instance.GetStoryProgressInfo();

            // Update StoryEvent
            (ParentEvent as StoryEvent).UpdateStoryProgress(storyInfo);
        }

        // Save user data
        DataManager.Instance.SaveUserData(slotNum);

        // Wait while saving
        while (DataManager.Instance.IsSaving)
        {
            yield return null;
        }

        // Turn Saving UI off
        UIController.Instance.TurnSubUIOff(SUBUI.SAVING);

        // Terminate data save event and play next event
        GameEventManager.Instance.TerminateGameEvent(this);
    }

    // Terminate data save event
    public override void TerminateEvent()
    {
        if (DataManager.Instance.IsSaving)
        {
            Debug.Log("Terminate error: data is not saved yet!");
        }
    }
}
