using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StoryModel : MonoBehaviour
{
	public static StoryModel Instance { get; private set; }

    private List<StoryObserver> observerList; // Observers which observes story queue
    private Queue<StoryBlock> storyBlockQueue = null;
    private Queue<StoryEntry> storyEntryQueue = null;
    private int readEntryCountBuffer = 0;

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
        StoryBlock firstBlock = storyBlockQueue.Dequeue(); // Get first story block
        CurrentStoryBranch = firstBlock.branchId; // Set current branch id according to the first story block
        storyEntryQueue = new Queue<StoryEntry>(firstBlock.entries.Skip(ReadEntryCount));


        // Notify observers about the update
        observerList.ForEach((obj) => obj.StoryUpdated());
    }

    // Return first story entry
    public StoryEntry GetFirstEntry()
    {
        return storyEntryQueue.Dequeue();
    }

    // Return next story entry in the queue.
    public StoryEntry GetNextEntry()
    {
        // Check if story queue is empty
        if (storyEntryQueue == null)
            return null;

        if (storyEntryQueue.Count > 0) // Get next entry from the storyEntryQueue
        {
            StoryEntry nextEntry = storyEntryQueue.Dequeue();
            readEntryCountBuffer++;

            if (nextEntry.savePoint) // If next entry is save point, update ReadEntryCount
            {
                ReadEntryCount = ReadEntryCount + readEntryCountBuffer;
                readEntryCountBuffer = 0;
            }

            return nextEntry; // Return next story entry
        }
        else
        {
            if(storyBlockQueue.Count > 0) // Move to next story block
            {
                // player read all entries in previous story block
                StoryBlock nextBlock = storyBlockQueue.Dequeue();
                ReadBlockCount++;
                ReadEntryCount = readEntryCountBuffer = 0;

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
                GameManager.Instance.PlayerData.ReadEntryCount = ReadEntryCount = readEntryCountBuffer = 0;

                GameEventRouter.Instance.EventOver();
                return null;
            }
        }
    }
}

public interface StoryObserver
{
    public void StoryUpdated();
}