using UnityEngine;
using UIEnums;
using EventEnums;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "NewDataSave", menuName = "Event/DataSaveEvent", order = 0)]
public class DataSaveEvent : GameEvent
{
    public int slotNum; // if 0 auto save, else save in slot
    private bool takeScreenShot = false;

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.DATA_SAVE;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
    {
        // Can be played when current base UI is loading or save
        if (baseUI == BaseUI.Loading || subUI == SubUI.Save)
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
        GameEventManager.Instance.StartCoroutine(PlayEventCoroutine());
    }
    public override IEnumerator PlayEventCoroutine()
    {
        // Turn Saving UI on
        UIController.Instance.TurnSubUIOn(SubUI.Saving);

        // When saving during story mode
        GameEvent rootEvent = GetRootEvent();
        if (rootEvent?.EventType == EVENT_TYPE.STORY)
        {
            // Get current story progress info
            var storyInfo = StoryController.Instance.GetStoryProgressInfo();

            // Update StoryEvent
            (rootEvent as StoryEvent).UpdateStoryProgress(storyInfo);

            // Take screen shot when saving
            takeScreenShot = true;
        }

        // Save user data
        GameEvent startingEvent = GetStartingEvent();
        DataManager.Instance.SaveUserData(startingEvent, slotNum, takeScreenShot);

        // Wait while saving
        while (DataManager.Instance.IsSaving)
        {
            yield return null;
        }

        // Turn Saving UI off
        UIController.Instance.TurnSubUIOff(SubUI.Saving);

        // Terminate data save event and play next event
        GameEventManager.Instance.TerminateGameEvent(this);
    }

    private GameEvent GetStartingEvent()
    {
        GameEvent startingEvent = GetRootEvent();
        if(startingEvent == null)
        {
            return NextEvent;
        }
        else
        {
            return startingEvent;
        }
    }

    private GameEvent GetRootEvent()
    {            
        if(ParentEvent == null)
        {
            return null;
        }
        else
        {
            GameEvent root = ParentEvent;
            while(root.ParentEvent != null)
            {
                root = root.ParentEvent;
            }
            return root;
        }
    }

    // Terminate data save event
    public override void TerminateEvent()
    {
        if (DataManager.Instance.IsSaving)
        {
            Debug.LogError("Terminate error: data is not saved yet!");
        }
    }
}
