using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StoryEditor
{
    [System.Serializable]
    public class EditorStoryScript
    {
        [SerializeField] private List<EditorStoryBlock> editorBlocks = new List<EditorStoryBlock>();
        [SerializeField] private int selectedBlockIndex = -1;
        [SerializeField] private int selectedEntryIndex = -1;

        public List<EditorStoryBlock> EditorBlocks => editorBlocks;
        public int SelectedBlockIndex 
        { 
            get => selectedBlockIndex;
            set => selectedBlockIndex = value;
        }
        public int SelectedEntryIndex 
        { 
            get => selectedEntryIndex;
            set => selectedEntryIndex = value;
        }

        public EditorStoryBlock SelectedBlock => 
            selectedBlockIndex >= 0 && selectedBlockIndex < editorBlocks.Count ? 
            editorBlocks[selectedBlockIndex] : null;

        public EditorStoryEntry SelectedEntry =>
            SelectedBlock?.SelectedEntry;

        public EditorStoryScript()
        {
            editorBlocks = new List<EditorStoryBlock>();
        }

        public EditorStoryScript(StoryScript storyScript)
        {
            LoadFromStoryScript(storyScript);
        }

        public void LoadFromStoryScript(StoryScript storyScript)
        {
            editorBlocks.Clear();
            selectedBlockIndex = -1;
            selectedEntryIndex = -1;

            if (storyScript?.Blocks != null)
            {
                foreach (var block in storyScript.Blocks)
                {
                    editorBlocks.Add(new EditorStoryBlock(block));
                }
            }
        }

        public StoryScript ToStoryScript()
        {
            var storyScript = new StoryScript
            {
                Blocks = editorBlocks.Select(eb => eb.ToStoryBlock()).ToList()
            };
            return storyScript;
        }

        public EditorStoryBlock AddBlock(string branchId = null)
        {
            if (string.IsNullOrEmpty(branchId))
            {
                branchId = $"Block{editorBlocks.Count + 1}";
            }

            var newBlock = new EditorStoryBlock(branchId);
            editorBlocks.Add(newBlock);
            return newBlock;
        }

        public bool RemoveBlock(int index)
        {
            if (index >= 0 && index < editorBlocks.Count)
            {
                editorBlocks.RemoveAt(index);
                if (selectedBlockIndex == index)
                {
                    selectedBlockIndex = -1;
                    selectedEntryIndex = -1;
                }
                else if (selectedBlockIndex > index)
                {
                    selectedBlockIndex--;
                }
                return true;
            }
            return false;
        }

        public bool MoveBlock(int fromIndex, int toIndex)
        {
            if (fromIndex >= 0 && fromIndex < editorBlocks.Count &&
                toIndex >= 0 && toIndex < editorBlocks.Count &&
                fromIndex != toIndex)
            {
                var block = editorBlocks[fromIndex];
                editorBlocks.RemoveAt(fromIndex);
                editorBlocks.Insert(toIndex, block);

                // Update selected index if needed
                if (selectedBlockIndex == fromIndex)
                {
                    selectedBlockIndex = toIndex;
                }
                else if (selectedBlockIndex > fromIndex && selectedBlockIndex <= toIndex)
                {
                    selectedBlockIndex--;
                }
                else if (selectedBlockIndex < fromIndex && selectedBlockIndex >= toIndex)
                {
                    selectedBlockIndex++;
                }

                return true;
            }
            return false;
        }

        public List<string> GetAllBranchIds()
        {
            return editorBlocks.Select(b => b.BranchId).ToList();
        }

        public List<string> GetAvailableBranchIds(int afterBlockIndex)
        {
            return editorBlocks
                .Skip(afterBlockIndex + 1)
                .Select(b => b.BranchId)
                .ToList();
        }

        public bool ValidateChoiceReferences()
        {
            for (int blockIndex = 0; blockIndex < editorBlocks.Count; blockIndex++)
            {
                var block = editorBlocks[blockIndex];
                var availableBranches = GetAvailableBranchIds(blockIndex);

                for (int entryIndex = 0; entryIndex < block.EditorEntries.Count; entryIndex++)
                {
                    var entry = block.EditorEntries[entryIndex];
                    if (entry.StoryEntry is StoryChoice choice)
                    {
                        foreach (var option in choice.Options)
                        {
                            if (!string.IsNullOrEmpty(option.BranchId) && 
                                !option.BranchId.Equals("common", System.StringComparison.OrdinalIgnoreCase) &&
                                !availableBranches.Contains(option.BranchId))
                            {
                                Debug.LogError($"Invalid branch reference '{option.BranchId}' in block '{block.BranchId}' entry {entryIndex + 1}");
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}