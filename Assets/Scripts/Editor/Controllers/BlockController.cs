using System.Collections.Generic;
using UnityEngine;

namespace StoryEditor.Controllers
{
    public class BlockController
    {
        private EditorStoryScript editorStoryScript;
        private System.Action onBlocksChanged;
        private System.Action onSelectionChanged;

        public BlockController(EditorStoryScript storyScript)
        {
            editorStoryScript = storyScript;
        }

        public void SetCallbacks(System.Action onBlocksChanged, System.Action onSelectionChanged)
        {
            this.onBlocksChanged = onBlocksChanged;
            this.onSelectionChanged = onSelectionChanged;
        }

        public EditorStoryBlock AddBlock(string branchName = null)
        {
            var newBlock = editorStoryScript.AddBlock(branchName);
            
            // Select the new block
            editorStoryScript.SelectedBlockIndex = editorStoryScript.EditorBlocks.Count - 1;
            editorStoryScript.SelectedEntryIndex = -1;

            onBlocksChanged?.Invoke();
            onSelectionChanged?.Invoke();

            return newBlock;
        }

        public bool RemoveBlock(int index)
        {
            Debug.Assert(0 <= index && index < editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (index < 0 || editorStoryScript.EditorBlocks.Count <= index)
                return false;

            var result = editorStoryScript.RemoveBlock(index);
            
            if (result)
            {
                onBlocksChanged?.Invoke();
                onSelectionChanged?.Invoke();
            }

            return result;
        }

        public bool RemoveSelectedBlock()
        {
            if (0 <= editorStoryScript.SelectedBlockIndex)
            {
                return RemoveBlock(editorStoryScript.SelectedBlockIndex);
            }
            return false;
        }

        public void SelectBlock(int index)
        {
            if (index >= -1 && index < editorStoryScript.EditorBlocks.Count)
            {
                editorStoryScript.SelectedBlockIndex = index;
                editorStoryScript.SelectedEntryIndex = -1;

                if (0 <= index)
                {
                    editorStoryScript.EditorBlocks[index].SelectedEntryIndex = -1;
                }

                onSelectionChanged?.Invoke();
            }
        }

        public void RenameBlock(int index, string newBranchName)
        {
            Debug.Assert(0 <= index && index < editorStoryScript.EditorBlocks.Count, "Block index out of range");
            Debug.Assert(false == string.IsNullOrWhiteSpace(newBranchName), "Branch name cannot be null or empty");
            if (index < 0 || editorStoryScript.EditorBlocks.Count <= index || string.IsNullOrWhiteSpace(newBranchName))
                return;
            
            editorStoryScript.EditorBlocks[index].BranchName = newBranchName;
            onBlocksChanged?.Invoke();
        }

        public void RenameSelectedBlock(string newBranchName)
        {
            if (0 <= editorStoryScript.SelectedBlockIndex)
            {
                RenameBlock(editorStoryScript.SelectedBlockIndex, newBranchName);
            }
        }

        public bool MoveBlock(int fromIndex, int toIndex)
        {
            var result = editorStoryScript.MoveBlock(fromIndex, toIndex);
            
            if (result)
            {
                onBlocksChanged?.Invoke();
                onSelectionChanged?.Invoke();
            }

            return result;
        }

        public bool CanMoveBlockUp(int index)
        {
            return 0 < index && editorStoryScript.EditorBlocks.Count > index;
        }

        public bool CanMoveBlockDown(int index)
        {
            return 0 <= index && editorStoryScript.EditorBlocks.Count - 1 > index;
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
            if (0 <= editorStoryScript.SelectedBlockIndex)
            {
                return MoveBlockUp(editorStoryScript.SelectedBlockIndex);
            }
            return false;
        }

        public bool MoveSelectedBlockDown()
        {
            if (0 <= editorStoryScript.SelectedBlockIndex)
            {
                return MoveBlockDown(editorStoryScript.SelectedBlockIndex);
            }
            return false;
        }

        public List<string> GetAllBranchNames()
        {
            return editorStoryScript.GetAllBranchNames();
        }

        public List<string> GetAvailableBranchNames(int afterBlockIndex)
        {
            return editorStoryScript.GetAvailableBranchNames(afterBlockIndex);
        }

        public bool ValidateBranchName(string branchName, int excludeIndex = -1)
        {
            if (string.IsNullOrWhiteSpace(branchName))
                return false;

            for (int i = 0; i < editorStoryScript.EditorBlocks.Count; i++)
            {
                if (i != excludeIndex && editorStoryScript.EditorBlocks[i].BranchName == branchName)
                {
                    return false;
                }
            }

            return true;
        }

        public string GenerateUniqueBranchName(string baseName = "Block")
        {
            int counter = 1;
            string candidateName;

            do
            {
                candidateName = $"{baseName}{counter}";
                counter++;
            }
            while (false == ValidateBranchName(candidateName));

            return candidateName;
        }

        public EditorStoryBlock GetSelectedBlock()
        {
            return editorStoryScript.SelectedBlock;
        }

        public int GetSelectedBlockIndex()
        {
            return editorStoryScript.SelectedBlockIndex;
        }

        public int GetBlockCount()
        {
            return editorStoryScript.EditorBlocks.Count;
        }

        public EditorStoryBlock GetBlock(int index)
        {
            Debug.Assert(0 <= index && index < editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (index < 0 || editorStoryScript.EditorBlocks.Count <= index)
                return null;
            
            return editorStoryScript.EditorBlocks[index];
        }
    }
}