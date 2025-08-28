using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.IO;

namespace AD.Story
{
    public class StoryModel : MonoBehaviour
    {
        /****** Public Members ******/

        public static StoryModel Instance { get; private set; }

        public int ReadBlockCount { get; set; } = 0;
        public int ReadEntryCount { get; set; } = 0;
        public string CurrentStoryBranch { get; set; }

        public Coroutine LoadStoryText(string storyInfo, int readBlockCount, int readEntryCount)
        {
            // Restore saved story point
            ReadBlockCount = readBlockCount;
            ReadEntryCount = readEntryCount;

            // Load Story Texts
            return StartCoroutine(AsyncLoadStoryText(storyInfo));
        }

        public StoryEntry GetFirstEntry()
        {
            StoryEntry firstEntry = storyEntryQueue.First.Value;
            storyEntryQueue.RemoveFirst();
            return firstEntry;
        }

        public StoryEntry PeekFirstEntry()
        {
            if (storyEntryQueue.Count > 0)
            {
                return storyEntryQueue.First.Value;
            }

            if (storyBlockQueue.Count > 0) // EntryQueue is empty <=> Block has completed.
            {
                foreach (var block in storyBlockQueue)
                {
                    if (block.IsCommon || block.BranchName == CurrentStoryBranch)
                    {
                        if (block.Entries.Count > 0)
                        {
                            return block.Entries[0];
                        }
                    }
                }
            }

            return null;
        }

        public StoryEntry GetNextEntry()
        {
            if (storyEntryQueue.Count > 0) // Get next entry from the storyEntryQueue
            {
                StoryEntry nextEntry = storyEntryQueue.First.Value;
                storyEntryQueue.RemoveFirst();
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
                        storyEntryQueue = new LinkedList<StoryEntry>(nextBlock.Entries); // Changed to LinkedList
                        StoryEntry nextEntry = storyEntryQueue.First.Value; // Get first entry from new block
                        storyEntryQueue.RemoveFirst(); // Remove it
                        return nextEntry;
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


        /****** Private Members ******/

        private Queue<StoryBlock> storyBlockQueue = null;
        private LinkedList<StoryEntry> storyEntryQueue = null; // Changed to LinkedList
        private int readEntryCountBuffer = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private IEnumerator AsyncLoadStoryText(string storyInfo)
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
            storyEntryQueue = new LinkedList<StoryEntry>(firstBlock.Entries.Skip(ReadEntryCount)); // Changed to LinkedList
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

    }
}