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
            0 <= selectedBlockIndex && selectedBlockIndex < editorBlocks.Count ? 
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

            if (null != storyScript?.Blocks)
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

        public EditorStoryBlock AddBlock(string branchName = null)
        {
            if (string.IsNullOrEmpty(branchName))
            {
                branchName = $"Block{editorBlocks.Count + 1}";
            }

            var newBlock = new EditorStoryBlock(branchName);
            editorBlocks.Add(newBlock);
            return newBlock;
        }

        public bool RemoveBlock(int index)
        {
            Debug.Assert(0 <= index && index < editorBlocks.Count, "Block index out of range");
            if (index < 0 || editorBlocks.Count <= index)
                return false;
            
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

        public bool MoveBlock(int fromIndex, int toIndex)
        {
            Debug.Assert(0 <= fromIndex && fromIndex < editorBlocks.Count, "From index out of range");
            Debug.Assert(0 <= toIndex && toIndex < editorBlocks.Count, "To index out of range");
            Debug.Assert(fromIndex != toIndex, "From and to indices cannot be the same");
            if (fromIndex < 0 || editorBlocks.Count <= fromIndex || toIndex < 0 || editorBlocks.Count <= toIndex || fromIndex == toIndex)
                return false;
            
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

        public List<string> GetAllBranchNames()
        {
            return editorBlocks.Select(b => b.BranchName).ToList();
        }

        public List<string> GetAvailableBranchNames(int afterBlockIndex)
        {
            return editorBlocks
                .Skip(afterBlockIndex + 1)
                .Select(b => b.BranchName)
                .ToList();
        }

        public bool ValidateChoiceReferences()
        {
            for (int blockIndex = 0; blockIndex < editorBlocks.Count; blockIndex++)
            {
                var block = editorBlocks[blockIndex];
                var availableBranches = GetAvailableBranchNames(blockIndex);

                for (int entryIndex = 0; entryIndex < block.EditorEntries.Count; entryIndex++)
                {
                    var entry = block.EditorEntries[entryIndex];
                    if (entry.StoryEntry is StoryChoice choice)
                    {
                        foreach (var option in choice.Options)
                        {
                            if (false == string.IsNullOrEmpty(option.BranchName) && 
                                false == StoryBlock.IsCommonBranch(option.BranchName) &&
                                false == availableBranches.Contains(option.BranchName))
                            {
                                Debug.LogError($"Invalid branch reference '{option.BranchName}' in block '{block.BranchName}' entry {entryIndex + 1}");
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