using System.Collections.Generic;
using UnityEngine;

namespace StoryEditor.Controllers
{
    public class BlockController
    {
        /****** Public Members ******/

        public BlockController(EditorStoryScript storyScript)
        {
            _editorStoryScript = storyScript;
        }

        public void SetCallbacks(System.Action onBlocksChanged, System.Action onSelectionChanged)
        {
            _onBlocksChanged = onBlocksChanged;
            _onSelectionChanged = onSelectionChanged;
        }

        public EditorStoryBlock AddBlock(string branchName = null)
        {
            var newBlock = _editorStoryScript.AddBlock(branchName);
            
            // Select the new block
            _editorStoryScript.SelectedBlockIndex = _editorStoryScript.EditorBlocks.Count - 1;
            _editorStoryScript.SelectedEntryIndex = -1;

            _onBlocksChanged?.Invoke();
            _onSelectionChanged?.Invoke();

            return newBlock;
        }

        public bool RemoveBlock(int index)
        {
            Debug.Assert(0 <= index && index < _editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (index < 0 || _editorStoryScript.EditorBlocks.Count <= index)
                return false;

            var result = _editorStoryScript.RemoveBlock(index);
            
            if (result)
            {
                _onBlocksChanged?.Invoke();
                _onSelectionChanged?.Invoke();
            }

            return result;
        }

        public bool RemoveSelectedBlock()
        {
            if (0 <= _editorStoryScript.SelectedBlockIndex)
            {
                return RemoveBlock(_editorStoryScript.SelectedBlockIndex);
            }
            return false;
        }

        public void SelectBlock(int index)
        {
            if (index >= -1 && index < _editorStoryScript.EditorBlocks.Count)
            {
                _editorStoryScript.SelectedBlockIndex = index;
                _editorStoryScript.SelectedEntryIndex = -1;

                if (0 <= index)
                {
                    _editorStoryScript.EditorBlocks[index].SelectedEntryIndex = -1;
                }

                _onSelectionChanged?.Invoke();
            }
        }

        public void RenameBlock(int index, string newBranchName)
        {
            Debug.Assert(0 <= index && index < _editorStoryScript.EditorBlocks.Count, "Block index out of range");
            Debug.Assert(false == string.IsNullOrWhiteSpace(newBranchName), "Branch name cannot be null or empty");
            if (index < 0 || _editorStoryScript.EditorBlocks.Count <= index || string.IsNullOrWhiteSpace(newBranchName))
                return;
            
            // Validate the new branch name (allows Common duplicates)
            if (false == ValidateBranchName(newBranchName, index))
            {
                Logger.Write(LogCategory.StoryScriptEditor, $"Branch name '{newBranchName}' already exists. Rename operation cancelled.", LogLevel.Warning);
                return;
            }
            
            _editorStoryScript.EditorBlocks[index].BranchName = newBranchName;
            _onBlocksChanged?.Invoke();
        }

        public void RenameSelectedBlock(string newBranchName)
        {
            if (0 <= _editorStoryScript.SelectedBlockIndex)
            {
                RenameBlock(_editorStoryScript.SelectedBlockIndex, newBranchName);
            }
        }

        public bool MoveBlock(int fromIndex, int toIndex)
        {
            var result = _editorStoryScript.MoveBlock(fromIndex, toIndex);
            
            if (result)
            {
                _onBlocksChanged?.Invoke();
                _onSelectionChanged?.Invoke();
            }

            return result;
        }

        public bool CanMoveBlockUp(int index)
        {
            return 0 < index && _editorStoryScript.EditorBlocks.Count > index;
        }

        public bool CanMoveBlockDown(int index)
        {
            return 0 <= index && _editorStoryScript.EditorBlocks.Count - 1 > index;
        }

        public bool MoveBlockUp(int index)
        {
            if (false == CanMoveBlockUp(index)) return false;
            return MoveBlock(index, index - 1);
        }

        public bool MoveBlockDown(int index)
        {
            if (false == CanMoveBlockDown(index)) return false;
            return MoveBlock(index, index + 1);
        }

        public bool MoveSelectedBlockUp()
        {
            if (0 <= _editorStoryScript.SelectedBlockIndex)
            {
                return MoveBlockUp(_editorStoryScript.SelectedBlockIndex);
            }
            return false;
        }

        public bool MoveSelectedBlockDown()
        {
            if (0 <= _editorStoryScript.SelectedBlockIndex)
            {
                return MoveBlockDown(_editorStoryScript.SelectedBlockIndex);
            }
            return false;
        }

        public List<string> GetAllBranchNames()
        {
            return _editorStoryScript.GetAllBranchNames();
        }

        public List<string> GetAvailableBranchNames(int afterBlockIndex)
        {
            return _editorStoryScript.GetAvailableBranchNames(afterBlockIndex);
        }

        public bool ValidateBranchName(string branchName, int excludeIndex = -1)
        {
            if (string.IsNullOrWhiteSpace(branchName))
                return false;

            // Common branches are always valid (allow duplicates)
            if (branchName == "Common")
                return true;

            for (int i = 0; i < _editorStoryScript.EditorBlocks.Count; i++)
            {
                if (i != excludeIndex && _editorStoryScript.EditorBlocks[i].BranchName == branchName)
                {
                    return false;
                }
            }

            return true;
        }


        public EditorStoryBlock GetSelectedBlock()
        {
            return _editorStoryScript.SelectedBlock;
        }

        public int GetSelectedBlockIndex()
        {
            return _editorStoryScript.SelectedBlockIndex;
        }

        public int GetBlockCount()
        {
            return _editorStoryScript.EditorBlocks.Count;
        }

        public EditorStoryBlock GetBlock(int index)
        {
            Debug.Assert(0 <= index && index < _editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (index < 0 || _editorStoryScript.EditorBlocks.Count <= index)
                return null;
            
            return _editorStoryScript.EditorBlocks[index];
        }


        /****** Private Members ******/

        private EditorStoryScript _editorStoryScript;
        private System.Action _onBlocksChanged;
        private System.Action _onSelectionChanged;
    }
}