using System.Collections.Generic;
using UnityEngine;

namespace StoryEditor.Controllers
{
    public enum EntryType
    {
        Dialogue,
        VFX,
        Choice,
        CharacterStanding,
        PlayMode,
        BackgroundCG,
        BGM,
        SFX,
        CameraAction
    }

    public class EntryController
    {
        private EditorStoryScript editorStoryScript;
        private System.Action onEntriesChanged;
        private System.Action onSelectionChanged;

        public EntryController(EditorStoryScript storyScript)
        {
            editorStoryScript = storyScript;
        }

        public void SetCallbacks(System.Action onEntriesChanged, System.Action onSelectionChanged)
        {
            this.onEntriesChanged = onEntriesChanged;
            this.onSelectionChanged = onSelectionChanged;
        }

        public EditorStoryEntry AddEntry(EntryType entryType, string character = "독백", string text = "", string action = "", float duration = 0f)
        {
            var selectedBlock = editorStoryScript.SelectedBlock;
            if (null == selectedBlock)
            {
                Debug.LogWarning("No block selected. Cannot add entry.");
                return null;
            }

            EditorStoryEntry newEntry = null;

            switch (entryType)
            {
                case EntryType.Dialogue:
                    newEntry = selectedBlock.AddDialogue(character, text);
                    break;
                case EntryType.VFX:
                    newEntry = selectedBlock.AddVFX(action, duration);
                    break;
                case EntryType.Choice:
                    newEntry = selectedBlock.AddChoice();
                    break;
                case EntryType.CharacterStanding:
                    newEntry = selectedBlock.AddCharacterStanding();
                    break;
                case EntryType.PlayMode:
                    newEntry = selectedBlock.AddPlayMode();
                    break;
                case EntryType.BackgroundCG:
                    newEntry = selectedBlock.AddBackgroundCG();
                    break;
                case EntryType.BGM:
                    newEntry = selectedBlock.AddBGM();
                    break;
                case EntryType.SFX:
                    newEntry = selectedBlock.AddSFX();
                    break;
                case EntryType.CameraAction:
                    newEntry = selectedBlock.AddCameraAction();
                    break;
            }

            if (null != newEntry)
            {
                // Select the new entry
                editorStoryScript.SelectedEntryIndex = selectedBlock.EditorEntries.Count - 1;
                selectedBlock.SelectedEntryIndex = editorStoryScript.SelectedEntryIndex;

                onEntriesChanged?.Invoke();
                onSelectionChanged?.Invoke();
            }

            return newEntry;
        }

        public bool RemoveEntry(int blockIndex, int entryIndex)
        {
            Debug.Assert(0 <= blockIndex && blockIndex < editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (blockIndex < 0 || editorStoryScript.EditorBlocks.Count <= blockIndex)
                return false;

            var block = editorStoryScript.EditorBlocks[blockIndex];
            Debug.Assert(0 <= entryIndex && entryIndex < block.EditorEntries.Count, "Entry index out of range");
            if (entryIndex < 0 || block.EditorEntries.Count <= entryIndex)
                return false;
            var result = block.RemoveEntry(entryIndex);

            if (result)
            {
                // Update selected entry index if needed
                if (editorStoryScript.SelectedBlockIndex == blockIndex)
                {
                    if (editorStoryScript.SelectedEntryIndex == entryIndex)
                    {
                        editorStoryScript.SelectedEntryIndex = -1;
                    }
                    else if (editorStoryScript.SelectedEntryIndex > entryIndex)
                    {
                        editorStoryScript.SelectedEntryIndex--;
                    }
                }

                onEntriesChanged?.Invoke();
                onSelectionChanged?.Invoke();
            }

            return result;
        }

        public bool RemoveSelectedEntry()
        {
            if (0 <= editorStoryScript.SelectedBlockIndex && 0 <= editorStoryScript.SelectedEntryIndex)
            {
                return RemoveEntry(editorStoryScript.SelectedBlockIndex, editorStoryScript.SelectedEntryIndex);
            }
            return false;
        }

        public void SelectEntry(int entryIndex)
        {
            var selectedBlock = editorStoryScript.SelectedBlock;
            if (null == selectedBlock) return;

            if (entryIndex >= -1 && entryIndex < selectedBlock.EditorEntries.Count)
            {
                editorStoryScript.SelectedEntryIndex = entryIndex;
                selectedBlock.SelectedEntryIndex = entryIndex;

                onSelectionChanged?.Invoke();
            }
        }

        public bool MoveEntry(int blockIndex, int fromIndex, int toIndex)
        {
            Debug.Assert(0 <= blockIndex && blockIndex < editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (blockIndex < 0 || editorStoryScript.EditorBlocks.Count <= blockIndex)
                return false;

            var block = editorStoryScript.EditorBlocks[blockIndex];
            Debug.Assert(0 <= fromIndex && fromIndex < block.EditorEntries.Count, "From index out of range");
            Debug.Assert(0 <= toIndex && toIndex < block.EditorEntries.Count, "To index out of range");
            if (fromIndex < 0 || block.EditorEntries.Count <= fromIndex || toIndex < 0 || block.EditorEntries.Count <= toIndex)
                return false;
            var result = block.MoveEntry(fromIndex, toIndex);

            if (result)
            {
                // Update selected entry index if needed
                if (editorStoryScript.SelectedBlockIndex == blockIndex)
                {
                    if (editorStoryScript.SelectedEntryIndex == fromIndex)
                    {
                        editorStoryScript.SelectedEntryIndex = toIndex;
                    }
                    else if (editorStoryScript.SelectedEntryIndex > fromIndex && 
                             editorStoryScript.SelectedEntryIndex <= toIndex)
                    {
                        editorStoryScript.SelectedEntryIndex--;
                    }
                    else if (editorStoryScript.SelectedEntryIndex < fromIndex && 
                             editorStoryScript.SelectedEntryIndex >= toIndex)
                    {
                        editorStoryScript.SelectedEntryIndex++;
                    }
                }

                onEntriesChanged?.Invoke();
                onSelectionChanged?.Invoke();
            }

            return result;
        }

        public bool CanMoveEntryUp(int blockIndex, int entryIndex)
        {
            if (blockIndex < 0 || editorStoryScript.EditorBlocks.Count <= blockIndex)
                return false;

            var block = editorStoryScript.EditorBlocks[blockIndex];
            return 0 < entryIndex && entryIndex < block.EditorEntries.Count;
        }

        public bool CanMoveEntryDown(int blockIndex, int entryIndex)
        {
            if (blockIndex < 0 || editorStoryScript.EditorBlocks.Count <= blockIndex)
                return false;

            var block = editorStoryScript.EditorBlocks[blockIndex];
            return 0 <= entryIndex && entryIndex < block.EditorEntries.Count - 1;
        }

        public bool MoveEntryUp(int blockIndex, int entryIndex)
        {
            if (false == CanMoveEntryUp(blockIndex, entryIndex)) return false;
            return MoveEntry(blockIndex, entryIndex, entryIndex - 1);
        }

        public bool MoveEntryDown(int blockIndex, int entryIndex)
        {
            if (false == CanMoveEntryDown(blockIndex, entryIndex)) return false;
            return MoveEntry(blockIndex, entryIndex, entryIndex + 1);
        }

        public bool MoveSelectedEntryUp()
        {
            if (0 <= editorStoryScript.SelectedBlockIndex && 0 <= editorStoryScript.SelectedEntryIndex)
            {
                return MoveEntryUp(editorStoryScript.SelectedBlockIndex, editorStoryScript.SelectedEntryIndex);
            }
            return false;
        }

        public bool MoveSelectedEntryDown()
        {
            if (0 <= editorStoryScript.SelectedBlockIndex && 0 <= editorStoryScript.SelectedEntryIndex)
            {
                return MoveEntryDown(editorStoryScript.SelectedBlockIndex, editorStoryScript.SelectedEntryIndex);
            }
            return false;
        }

        public void UpdateChoiceEntry(EditorStoryEntry choiceEntry, int blockIndex, int entryIndex)
        {
            if (null != choiceEntry && choiceEntry.IsChoice())
            {
                var block = editorStoryScript.EditorBlocks[blockIndex];
                choiceEntry.UpdateChoicePrevDialogue(block, entryIndex);
                onEntriesChanged?.Invoke();
            }
        }

        public void UpdateAllChoiceEntriesInBlock(int blockIndex)
        {
            Debug.Assert(0 <= blockIndex && blockIndex < editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (blockIndex < 0 || editorStoryScript.EditorBlocks.Count <= blockIndex)
                return;

            var block = editorStoryScript.EditorBlocks[blockIndex];
            bool hasChanges = false;

            for (int i = 0; i < block.EditorEntries.Count; i++)
            {
                var entry = block.EditorEntries[i];
                if (entry.IsChoice())
                {
                    entry.UpdateChoicePrevDialogue(block, i);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                onEntriesChanged?.Invoke();
            }
        }

        public void AddChoiceOption(EditorStoryEntry choiceEntry, string branchName = null, string text = "")
        {
            Debug.Assert(null != choiceEntry, "Choice entry cannot be null");
            Debug.Assert(choiceEntry.IsChoice(), "Entry must be a choice type");
            
            var choice = choiceEntry.AsChoice();
            if (null == choice.Options)
            {
                choice.Options = new List<StoryChoiceOption>();
            }

            choice.Options.Add(new StoryChoiceOption
            {
                BranchName = branchName ?? StoryBlock.CommonBranch,
                Text = text
            });

            onEntriesChanged?.Invoke();
        }

        public void RemoveChoiceOption(EditorStoryEntry choiceEntry, int optionIndex)
        {
            Debug.Assert(null != choiceEntry, "Choice entry cannot be null");
            Debug.Assert(choiceEntry.IsChoice(), "Entry must be a choice type");
            
            var choice = choiceEntry.AsChoice();
            Debug.Assert(null != choice.Options, "Choice options cannot be null");
            Debug.Assert(0 <= optionIndex && optionIndex < choice.Options.Count, "Option index out of range");
            
            choice.Options.RemoveAt(optionIndex);
            onEntriesChanged?.Invoke();
        }

        public EditorStoryEntry GetSelectedEntry()
        {
            return editorStoryScript.SelectedEntry;
        }

        public int GetSelectedEntryIndex()
        {
            return editorStoryScript.SelectedEntryIndex;
        }

        public List<EditorStoryEntry> GetEntriesForSelectedBlock()
        {
            return editorStoryScript.SelectedBlock?.EditorEntries ?? new List<EditorStoryEntry>();
        }

        public bool ValidateEntry(int blockIndex, int entryIndex)
        {
            Debug.Assert(0 <= blockIndex && blockIndex < editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (blockIndex < 0 || editorStoryScript.EditorBlocks.Count <= blockIndex)
                return false;

            var block = editorStoryScript.EditorBlocks[blockIndex];
            Debug.Assert(0 <= entryIndex && entryIndex < block.EditorEntries.Count, "Entry index out of range");
            if (entryIndex < 0 || block.EditorEntries.Count <= entryIndex)
                return false;

            var entry = block.EditorEntries[entryIndex];
            return entry.ValidateEntry(block, entryIndex);
        }
    }
}