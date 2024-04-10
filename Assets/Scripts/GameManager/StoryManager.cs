using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StoryManager : MonoBehaviour
{
	public static StoryManager Instance { get; private set; }

    private List<StoryObserver> observerList; // Observers which observes story queue
    private Queue<StoryBlock> storyBlockQueue = null;
    private Queue<StoryEntry> storyEntryQueue = null;

    public int ReadBlockCount { get; set; } = 0;
    public int ReadEntryCount { get; set; } = 0;
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
    public void SetStoryInfo(List<StoryBlock> storyBlocks)
    {
        // Restore saved story point
        ReadBlockCount = GameManager.Instance.PlayerData.ReadBlockCount;
        ReadEntryCount = GameManager.Instance.PlayerData.ReadEntryCount;
        storyBlockQueue = new Queue<StoryBlock>(storyBlocks.Skip(ReadBlockCount));
        CurrentStoryBranch = storyBlockQueue.Peek().branchId; // Set current branch id
        storyEntryQueue = new Queue<StoryEntry>(storyBlockQueue.Dequeue().entries.Skip(ReadEntryCount));

        // Notify observers about the update
        observerList.ForEach((obj) => obj.StoryUpdated());
    }

    // Return next story entry in the queue.
    public StoryEntry GetNextEntry()
    {
        // Check if story queue is empty
        if (storyEntryQueue == null)
            return null;

        if (storyEntryQueue.Count > 0)
            return storyEntryQueue.Dequeue(); // Return next story entry
        else
        {
            if(storyBlockQueue.Count > 0) // Move to next story block
            {
                // player read all entries in previous story block
                StoryBlock nextBlock = storyBlockQueue.Dequeue();
                ReadBlockCount++;

                // Play next story block when it is common or is same with current story branch
                if (nextBlock.branchId == "common" || nextBlock.branchId == CurrentStoryBranch)
                {
                    CurrentStoryBranch = nextBlock.branchId;
                    storyEntryQueue = new Queue<StoryEntry>(nextBlock.entries);
                    return storyEntryQueue.Dequeue();
                }
                else // Skip to next block
                {
                    return GetNextEntry();
                }
            }
            else // End Story event
            {
                // Reset read dialogue number to 0
                GameManager.Instance.PlayerData.ReadBlockCount = ReadBlockCount = 0;
                GameManager.Instance.PlayerData.ReadEntryCount = ReadEntryCount = 0;

                EventManager.Instance.EventOver();
                return null;
            }
        }
    }
}

public interface StoryObserver
{
    public void StoryUpdated();
}