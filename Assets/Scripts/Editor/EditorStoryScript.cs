using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AD.Story;

namespace StoryEditor
{
    [System.Serializable]
    public class EditorStoryScript
    {
        /****** Public Members ******/

        public List<EditorStoryBlock> EditorBlocks => _editorBlocks;
        public int SelectedBlockIndex
        {
            get => _selectedBlockIndex;
            set => _selectedBlockIndex = value;
        }
        public int SelectedEntryIndex
        {
            get => _selectedEntryIndex;
            set => _selectedEntryIndex = value;
        }

        public EditorStoryBlock SelectedBlock =>
            0 <= _selectedBlockIndex && _selectedBlockIndex < _editorBlocks.Count ?
            _editorBlocks[_selectedBlockIndex] : null;

        public EditorStoryEntry SelectedEntry =>
            SelectedBlock?.SelectedEntry;

        public EditorStoryScript()
        {
            _editorBlocks = new List<EditorStoryBlock>();
        }

        public EditorStoryScript(StoryScript storyScript)
        {
            LoadFromStoryScript(storyScript);
        }

        public void LoadFromStoryScript(StoryScript storyScript)
        {
            _editorBlocks.Clear();
            _selectedBlockIndex = -1;
            _selectedEntryIndex = -1;

            if (null != storyScript?.Blocks)
            {
                foreach (var block in storyScript.Blocks)
                {
                    _editorBlocks.Add(new EditorStoryBlock(block));
                }
            }
        }

        public StoryScript ToStoryScript()
        {
            var storyScript = new StoryScript
            {
                Blocks = _editorBlocks.Select(eb => eb.ToStoryBlock()).ToList()
            };
            return storyScript;
        }

        public EditorStoryBlock AddBlock(string branchName = null)
        {
            if (string.IsNullOrEmpty(branchName))
            {
                branchName = "Common";
            }
            else if (branchName != "Common")
            {
                // Only generate unique names for non-Common branches
                branchName = GenerateUniqueBranchName(branchName);
            }

            var newBlock = new EditorStoryBlock(branchName);
            _editorBlocks.Add(newBlock);
            return newBlock;
        }

        public bool RemoveBlock(int index)
        {
            Debug.Assert(0 <= index && index < _editorBlocks.Count, "Block index out of range");
            if (index < 0 || _editorBlocks.Count <= index)
                return false;

            _editorBlocks.RemoveAt(index);
            if (_selectedBlockIndex == index)
            {
                _selectedBlockIndex = -1;
                _selectedEntryIndex = -1;
            }
            else if (_selectedBlockIndex > index)
            {
                _selectedBlockIndex--;
            }
            return true;
        }

        public bool MoveBlock(int fromIndex, int toIndex)
        {
            Debug.Assert(0 <= fromIndex && fromIndex < _editorBlocks.Count, "From index out of range");
            Debug.Assert(0 <= toIndex && toIndex < _editorBlocks.Count, "To index out of range");
            if (fromIndex < 0 || _editorBlocks.Count <= fromIndex || toIndex < 0 || _editorBlocks.Count <= toIndex || fromIndex == toIndex)
                return false;

            var block = _editorBlocks[fromIndex];
            _editorBlocks.RemoveAt(fromIndex);
            _editorBlocks.Insert(toIndex, block);

            // Update selected index if needed
            if (_selectedBlockIndex == fromIndex)
            {
                _selectedBlockIndex = toIndex;
            }
            else if (_selectedBlockIndex > fromIndex && _selectedBlockIndex <= toIndex)
            {
                _selectedBlockIndex--;
            }
            else if (_selectedBlockIndex < fromIndex && _selectedBlockIndex >= toIndex)
            {
                _selectedBlockIndex++;
            }

            return true;
        }

        public List<string> GetAllBranchNames()
        {
            return _editorBlocks.Select(b => b.BranchName).ToList();
        }

        public List<string> GetAvailableBranchNames(int afterBlockIndex)
        {
            return _editorBlocks
                .Skip(afterBlockIndex + 1)
                .Select(b => b.BranchName)
                .ToList();
        }

        public bool ValidateChoiceReferences()
        {
            for (int blockIndex = 0; blockIndex < _editorBlocks.Count; blockIndex++)
            {
                var block = _editorBlocks[blockIndex];
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
                                Logger.Write(LogCategory.StoryScriptEditor, $"Invalid branch reference '{option.BranchName}' in block '{block.BranchName}' entry {entryIndex + 1}", LogLevel.Error);
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }


        /****** Private Members ******/

        [SerializeField] private List<EditorStoryBlock> _editorBlocks = new List<EditorStoryBlock>();
        [SerializeField] private int _selectedBlockIndex = -1;
        [SerializeField] private int _selectedEntryIndex = -1;
        
        private string GenerateUniqueBranchName(string baseName)
        {
            // Common branches are always allowed to have duplicate names
            if (baseName == "Common")
            {
                return baseName;
            }

            // First try the base name without any suffix
            if (IsUniqueBranchName(baseName))
            {
                return baseName;
            }

            // If not unique, try with numbers
            int counter = 1;
            string candidateName;
            do
            {
                candidateName = $"{baseName}{counter}";
                counter++;
            }
            while (false == IsUniqueBranchName(candidateName));

            return candidateName;
        }

        private bool IsUniqueBranchName(string branchName)
        {
            // Common branches are always considered unique (allow duplicates)
            if (branchName == "Common")
            {
                return true;
            }
            
            return _editorBlocks.All(block => block.BranchName != branchName);
        }
    }
}