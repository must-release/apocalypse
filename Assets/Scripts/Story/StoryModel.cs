using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.IO;

public class StoryModel : MonoBehaviour
{
    public static StoryModel Instance { get; private set; }

    private Queue<StoryBlock> storyBlockQueue = null;
    private Queue<StoryEntry> storyEntryQueue = null;
    private int readEntryCountBuffer = 0;

    public int ReadBlockCount { get; set; } = 0;
    public int ReadEntryCount { get; set; } = 0;
    public string CurrentStoryBranch { get; private set; }
    public Queue<StoryEntry> StoryEntryBuffer { get; set; }
    public StoryChoice ProcessingChoice { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StoryEntryBuffer = new Queue<StoryEntry>();
        }
    }

    // Start loading text of the current story event which player is having 
    public Coroutine LoadStoryText(string storyInfo, int readBlockCount, int readEntryCount)
    {
        // Restore saved story point
        ReadBlockCount = readBlockCount;
        ReadEntryCount = readEntryCount;

        // Load Story Texts
        return StartCoroutine(AsyncLoadStoryText(storyInfo));
    }

    IEnumerator AsyncLoadStoryText(string storyInfo)
    {
        AsyncOperationHandle<TextAsset> story = Addressables.LoadAssetAsync<TextAsset>(storyInfo);
        yield return story;

        if (story.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Failed to load story: {storyInfo}. Status: {story.Status}");
            yield break; // Exit if loading failed
        }

        // Convert XML file to StoryBlocks object
        string xmlContent = story.Result.text;
        StoryScript storyBlocks = DeserializeFromXml<StoryScript>(xmlContent);

        // Set story info
        SetStoryInfo(storyBlocks.Blocks);
    }

    // Set story information
    private void SetStoryInfo(List<StoryBlock> storyBlocks)
    {
        storyBlockQueue = new Queue<StoryBlock>(storyBlocks.Skip(ReadBlockCount));
        StoryBlock firstBlock = storyBlockQueue.Dequeue(); // Get first story block
        CurrentStoryBranch = firstBlock.BranchName; // Set current branch name according to the first story block
        storyEntryQueue = new Queue<StoryEntry>(firstBlock.Entries.Skip(ReadEntryCount));
    }

    // Deserialize XML string to object
    private T DeserializeFromXml<T>(string xmlContent)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (StringReader reader = new StringReader(xmlContent))
        {
            return (T)serializer.Deserialize(reader);
        }
    }

    // Return first story entry
    public StoryEntry GetFirstEntry()
    {
        return storyEntryQueue.Dequeue();
    }

    // Set current branch according to selected choice option
    public void SetCurrentBranch(string optionText)
    {
        if (ProcessingChoice != null && ProcessingChoice.Options != null)
        {
            foreach (var option in ProcessingChoice.Options)
            {
                if (option.Text.Equals(optionText))
                {
                    CurrentStoryBranch = option.BranchName;
                    break;
                }
            }
        }
    }

    // Return next story entry in the queue.
    public StoryEntry GetNextEntry()
    {
        if (StoryEntryBuffer.Count > 0) // return buffered entry
        {
            return StoryEntryBuffer.Dequeue();
        }
        else if (storyEntryQueue.Count > 0) // Get next entry from the storyEntryQueue
        {
            StoryEntry nextEntry = storyEntryQueue.Dequeue();
            readEntryCountBuffer++;

            if (nextEntry.IsSavePoint) // If next entry is save point, update ReadEntryCount
            {
                ReadEntryCount += readEntryCountBuffer;
                readEntryCountBuffer = 0;
            }

            return nextEntry; // Return next story entry
        }
        else
        {
            if (storyBlockQueue.Count > 0) // Movement to next story block
            {
                // player read all entries in previous story block
                StoryBlock nextBlock = storyBlockQueue.Dequeue();
                ReadBlockCount++;
                ReadEntryCount = readEntryCountBuffer = 0;

                // Play next story block when it is common or is same with current story branch
                if (nextBlock.IsCommon || nextBlock.BranchName == CurrentStoryBranch)
                {
                    CurrentStoryBranch = nextBlock.BranchName;
                    storyEntryQueue = new Queue<StoryEntry>(nextBlock.Entries);
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
                ReadBlockCount = 0;
                ReadEntryCount = readEntryCountBuffer = 0;

                return null;
            }
        }
    }
}
