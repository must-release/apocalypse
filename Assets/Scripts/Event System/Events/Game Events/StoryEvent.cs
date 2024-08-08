using UnityEngine;
using StageEnums;
using UIEnums;
using EventEnums;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "NewStory", menuName = "Event/StoryEvent", order = 0)]
public class StoryEvent : GameEvent
{
    public STAGE stage;
    public int storyNum;
    public int readBlockCount;
    public int readEntryCount;
    public bool onMap; // If story is played on the map

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.STORY;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, BASEUI baseUI, SUBUI subUI)
    {
        // Can be played only when there is no event playing
        if (parentEvent == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Play Story Event
    public override void PlayEvent()
    {
        // Use GameEventManger to start coroutine
        GameEventManager.Instance.StartCoroutineForGameEvents(PlayEventCoroutine());
    }
    IEnumerator PlayEventCoroutine()
    {
        // Change to story UI
        UIController.Instance.ChangeBaseUI(BASEUI.STORY);

        // Start Story
        InputEventProducer.Instance.LockInput(true);
        string story = "STORY_" + stage.ToString() + '_' + storyNum;
        yield return StoryController.Instance.StartStory(story, readBlockCount, readEntryCount);
        InputEventProducer.Instance.LockInput(false);

        // Wait for story to end
        while (StoryController.Instance.IsStoryPlaying)
        {
            yield return null;
        }

        // Terminate story event and play next event
        GameEventManager.Instance.TerminateGameEvent(this);
    }

    // Update current story progress info
    public void UpdateStoryProgress((int, int) storyInfo)
    {
        readBlockCount = storyInfo.Item1;
        readEntryCount = storyInfo.Item2;
    }

    // Terminate story event
    public override void TerminateEvent()
    {
        // Tell StoryController to finish story
        StoryController.Instance.FinishStory();
    }
}
