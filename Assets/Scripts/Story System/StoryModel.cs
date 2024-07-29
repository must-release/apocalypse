using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class StoryModel : MonoBehaviour
{
	public static StoryModel Instance { get; private set; }

    private Queue<StoryBlock> storyBlockQueue = null;
    private Queue<StoryEntry> storyEntryQueue = null;
    private int readEntryCountBuffer = 0;

    public int ReadBlockCount { get; set; } = 0;
    public int ReadEntryCount { get; set; } = 0;
    public string CurrentStoryBranch { get; set; }
    public Queue<StoryEntry> storyEntryBuffer;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            storyEntryBuffer = new Queue<StoryEntry>();
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
        // Set JsonConvert settings
        var settings = new JsonSerializerSettings
        {
            // Add custom converter
            Converters = new List<JsonConverter> { new StoryEntryConverter() }
        };

        // Load story
        AsyncOperationHandle<TextAsset> story = Addressables.LoadAssetAsync<TextAsset>(storyInfo);
        yield return story;

        // Convert Json file to StoryEntries object
        string jsonContent = story.Result.text;
        StoryBlocks storyBlocks = JsonConvert.DeserializeObject<StoryBlocks>(jsonContent, settings);

        // Set story info
        SetStoryInfo(storyBlocks.blocks);
    }

    // Set story information
    public void SetStoryInfo(List<StoryBlock> storyBlocks)
    {
        storyBlockQueue = new Queue<StoryBlock>(storyBlocks.Skip(ReadBlockCount));
        StoryBlock firstBlock = storyBlockQueue.Dequeue(); // Get first story block
        CurrentStoryBranch = firstBlock.branchId; // Set current branch id according to the first story block
        storyEntryQueue = new Queue<StoryEntry>(firstBlock.entries.Skip(ReadEntryCount));

    }

    // Return first story entry
    public StoryEntry GetFirstEntry()
    {
        return storyEntryQueue.Dequeue();
    }

    // Return next story entry in the queue.
    public StoryEntry GetNextEntry()
    {
        if(storyEntryBuffer.Count > 0)
        {
            return storyEntryBuffer.Dequeue();
        }

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
                ReadBlockCount = 0;
                ReadEntryCount = readEntryCountBuffer = 0;

                //GameEventManager.Instance.TerminateEvent();
                return null;
            }
        }
    }

    // Class for converting story json file
    class StoryEntryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(StoryEntry));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            switch (jo["type"].Value<string>())
            {
                case "dialogue":
                    return jo.ToObject<Dialogue>(serializer);
                case "effect":
                    return jo.ToObject<Effect>(serializer);
                case "choice":
                    return jo.ToObject<Choice>(serializer);
                default:
                    throw new Exception("Unknown type");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}
