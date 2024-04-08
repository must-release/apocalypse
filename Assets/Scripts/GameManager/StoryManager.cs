using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StoryManager : MonoBehaviour
{
	public static StoryManager Instance { get; private set; }

    private List<StoryObserver> observerList; // Observers which observes story queue
    private Queue<StoryEntry> storyQueue = null;

    public int ReadDialogueCount { get; set; } = 0;
    public string CurrentStoryBranch { get; set; }


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            observerList = new List<StoryObserver>();
        }
    }

    // Add new observer to the list
    public void AddObserver(StoryObserver newObserver)
    {
        observerList.Add(newObserver);
    }

    // Set story information
    public void SetStoryInfo(List<StoryEntry> storyEntries)
    {
        ReadDialogueCount = GameManager.Instance.PlayerData.ReadDialogueCount;
        storyQueue = new Queue<StoryEntry>(storyEntries.Skip(ReadDialogueCount));

        // Notify observers about the update
        observerList.ForEach((obj) => obj.StoryUpdated());
    }

    // Return next story entry in the queue.
    public StoryEntry GetNextEntry()
    {
        // Check if story queue is empty
        if (storyQueue == null)
            return null;

        if (storyQueue.Count > 0)
            return storyQueue.Dequeue();
        else
        {
            // Reset read dialogue number to 0
            GameManager.Instance.PlayerData.ReadDialogueCount = 0;
            ReadDialogueCount = 0;

            EventManager.Instance.EventOver();
            return null;
        }
    }

    // Play next script on the screen
    public void PlayNextScript()
    {
        StoryEntry entry = GetNextEntry();
        if (entry == null)
        {
            return;
        }
        ReadDialogueCount++;
        StoryPlayer.Instance.ShowStoryEntry(entry);
    }
}

public interface StoryObserver
{
    public void StoryUpdated();
}