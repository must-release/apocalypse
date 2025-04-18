﻿using UnityEngine;
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
    public Choice ProcessingChoice { get; set; }

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
        // Load story
        AsyncOperationHandle<TextAsset> story = Addressables.LoadAssetAsync<TextAsset>(storyInfo);
        yield return story;

        // Convert XML file to StoryBlocks object
        string xmlContent = story.Result.text;
        StoryBlocks storyBlocks = DeserializeFromXml<StoryBlocks>(xmlContent);

        // Set story info
        SetStoryInfo(storyBlocks.blocks);
    }

    // Set story information
    private void SetStoryInfo(List<StoryBlock> storyBlocks)
    {
        storyBlockQueue = new Queue<StoryBlock>(storyBlocks.Skip(ReadBlockCount));
        StoryBlock firstBlock = storyBlockQueue.Dequeue(); // Get first story block
        CurrentStoryBranch = firstBlock.branchId; // Set current branch id according to the first story block
        storyEntryQueue = new Queue<StoryEntry>(firstBlock.entries.Skip(ReadEntryCount));
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
        if (ProcessingChoice != null && ProcessingChoice.options != null)
        {
            foreach (var option in ProcessingChoice.options)
            {
                if (option.text.Equals(optionText))
                {
                    CurrentStoryBranch = option.branchId;
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

            if (nextEntry.isSavePoint) // If next entry is save point, update ReadEntryCount
            {
                ReadEntryCount += readEntryCountBuffer;
                readEntryCountBuffer = 0;
            }

            return nextEntry; // Return next story entry
        }
        else
        {
            if (storyBlockQueue.Count > 0) // Move to next story block
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
                ReadBlockCount = 0;
                ReadEntryCount = readEntryCountBuffer = 0;

                return null;
            }
        }
    }
}
