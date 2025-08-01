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

        public EditorStoryBlock AddBlock(string branchId = null)
        {
            var newBlock = editorStoryScript.AddBlock(branchId);
            
            // Select the new block
            editorStoryScript.SelectedBlockIndex = editorStoryScript.EditorBlocks.Count - 1;
            editorStoryScript.SelectedEntryIndex = -1;

            onBlocksChanged?.Invoke();
            onSelectionChanged?.Invoke();

            return newBlock;
        }

        public bool RemoveBlock(int index)
        {
            if (index < 0 || index >= editorStoryScript.EditorBlocks.Count)
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
            if (editorStoryScript.SelectedBlockIndex >= 0)
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

                if (index >= 0)
                {
                    editorStoryScript.EditorBlocks[index].SelectedEntryIndex = -1;
                }

                onSelectionChanged?.Invoke();
            }
        }

        public void RenameBlock(int index, string newBranchId)
        {
            if (index >= 0 && index < editorStoryScript.EditorBlocks.Count)
            {
                editorStoryScript.EditorBlocks[index].BranchId = newBranchId;
                onBlocksChanged?.Invoke();
            }
        }

        public void RenameSelectedBlock(string newBranchId)
        {
            if (editorStoryScript.SelectedBlockIndex >= 0)
            {
                RenameBlock(editorStoryScript.SelectedBlockIndex, newBranchId);
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
            return index > 0 && index < editorStoryScript.EditorBlocks.Count;
        }

        public bool CanMoveBlockDown(int index)
        {
            return index >= 0 && index < editorStoryScript.EditorBlocks.Count - 1;
        }

        public bool MoveBlockUp(int index)
        {
            if (!CanMoveBlockUp(index)) return false;
            return MoveBlock(index, index - 1);
        }

        public bool MoveBlockDown(int index)
        {
            if (!CanMoveBlockDown(index)) return false;
            return MoveBlock(index, index + 1);
        }

        public bool MoveSelectedBlockUp()
        {
            if (editorStoryScript.SelectedBlockIndex >= 0)
            {
                return MoveBlockUp(editorStoryScript.SelectedBlockIndex);
            }
            return false;
        }

        public bool MoveSelectedBlockDown()
        {
            if (editorStoryScript.SelectedBlockIndex >= 0)
            {
                return MoveBlockDown(editorStoryScript.SelectedBlockIndex);
            }
            return false;
        }

        public List<string> GetAllBranchIds()
        {
            return editorStoryScript.GetAllBranchIds();
        }

        public List<string> GetAvailableBranchIds(int afterBlockIndex)
        {
            return editorStoryScript.GetAvailableBranchIds(afterBlockIndex);
        }

        public bool ValidateBranchId(string branchId, int excludeIndex = -1)
        {
            if (string.IsNullOrWhiteSpace(branchId))
                return false;

            for (int i = 0; i < editorStoryScript.EditorBlocks.Count; i++)
            {
                if (i != excludeIndex && editorStoryScript.EditorBlocks[i].BranchId == branchId)
                {
                    return false;
                }
            }

            return true;
        }

        public string GenerateUniqueBranchId(string baseName = "Block")
        {
            int counter = 1;
            string candidateId;

            do
            {
                candidateId = $"{baseName}{counter}";
                counter++;
            }
            while (!ValidateBranchId(candidateId));

            return candidateId;
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
            if (index >= 0 && index < editorStoryScript.EditorBlocks.Count)
            {
                return editorStoryScript.EditorBlocks[index];
            }
            return null;
        }
    }
}