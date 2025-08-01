using System.Collections.Generic;
using UnityEngine;

namespace StoryEditor.Controllers
{
    public enum EntryType
    {
        Dialogue,
        VFX,
        Choice,
        CharacterStanding
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
            if (selectedBlock == null)
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
                    // Note: StoryCharacterStanding is not fully implemented in the original interfaces
                    // This would need to be implemented based on the actual requirements
                    Debug.LogWarning("CharacterStanding entry type not fully implemented");
                    break;
            }

            if (newEntry != null)
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
            if (blockIndex < 0 || blockIndex >= editorStoryScript.EditorBlocks.Count)
                return false;

            var block = editorStoryScript.EditorBlocks[blockIndex];
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
            if (editorStoryScript.SelectedBlockIndex >= 0 && editorStoryScript.SelectedEntryIndex >= 0)
            {
                return RemoveEntry(editorStoryScript.SelectedBlockIndex, editorStoryScript.SelectedEntryIndex);
            }
            return false;
        }

        public void SelectEntry(int entryIndex)
        {
            var selectedBlock = editorStoryScript.SelectedBlock;
            if (selectedBlock == null) return;

            if (entryIndex >= -1 && entryIndex < selectedBlock.EditorEntries.Count)
            {
                editorStoryScript.SelectedEntryIndex = entryIndex;
                selectedBlock.SelectedEntryIndex = entryIndex;

                onSelectionChanged?.Invoke();
            }
        }

        public bool MoveEntry(int blockIndex, int fromIndex, int toIndex)
        {
            if (blockIndex < 0 || blockIndex >= editorStoryScript.EditorBlocks.Count)
                return false;

            var block = editorStoryScript.EditorBlocks[blockIndex];
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
            if (blockIndex < 0 || blockIndex >= editorStoryScript.EditorBlocks.Count)
                return false;

            var block = editorStoryScript.EditorBlocks[blockIndex];
            return entryIndex > 0 && entryIndex < block.EditorEntries.Count;
        }

        public bool CanMoveEntryDown(int blockIndex, int entryIndex)
        {
            if (blockIndex < 0 || blockIndex >= editorStoryScript.EditorBlocks.Count)
                return false;

            var block = editorStoryScript.EditorBlocks[blockIndex];
            return entryIndex >= 0 && entryIndex < block.EditorEntries.Count - 1;
        }

        public bool MoveEntryUp(int blockIndex, int entryIndex)
        {
            if (!CanMoveEntryUp(blockIndex, entryIndex)) return false;
            return MoveEntry(blockIndex, entryIndex, entryIndex - 1);
        }

        public bool MoveEntryDown(int blockIndex, int entryIndex)
        {
            if (!CanMoveEntryDown(blockIndex, entryIndex)) return false;
            return MoveEntry(blockIndex, entryIndex, entryIndex + 1);
        }

        public bool MoveSelectedEntryUp()
        {
            if (editorStoryScript.SelectedBlockIndex >= 0 && editorStoryScript.SelectedEntryIndex >= 0)
            {
                return MoveEntryUp(editorStoryScript.SelectedBlockIndex, editorStoryScript.SelectedEntryIndex);
            }
            return false;
        }

        public bool MoveSelectedEntryDown()
        {
            if (editorStoryScript.SelectedBlockIndex >= 0 && editorStoryScript.SelectedEntryIndex >= 0)
            {
                return MoveEntryDown(editorStoryScript.SelectedBlockIndex, editorStoryScript.SelectedEntryIndex);
            }
            return false;
        }

        public void UpdateChoiceEntry(EditorStoryEntry choiceEntry, int blockIndex, int entryIndex)
        {
            if (choiceEntry != null && choiceEntry.IsChoice())
            {
                var block = editorStoryScript.EditorBlocks[blockIndex];
                choiceEntry.UpdateChoicePrevDialogue(block, entryIndex);
                onEntriesChanged?.Invoke();
            }
        }

        public void AddChoiceOption(EditorStoryEntry choiceEntry, string branchId = "common", string text = "")
        {
            if (choiceEntry != null && choiceEntry.IsChoice())
            {
                var choice = choiceEntry.AsChoice();
                if (choice.Options == null)
                {
                    choice.Options = new List<StoryChoiceOption>();
                }

                choice.Options.Add(new StoryChoiceOption
                {
                    BranchId = branchId,
                    Text = text
                });

                onEntriesChanged?.Invoke();
            }
        }

        public void RemoveChoiceOption(EditorStoryEntry choiceEntry, int optionIndex)
        {
            if (choiceEntry != null && choiceEntry.IsChoice())
            {
                var choice = choiceEntry.AsChoice();
                if (choice.Options != null && optionIndex >= 0 && optionIndex < choice.Options.Count)
                {
                    choice.Options.RemoveAt(optionIndex);
                    onEntriesChanged?.Invoke();
                }
            }
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
            if (blockIndex < 0 || blockIndex >= editorStoryScript.EditorBlocks.Count)
                return false;

            var block = editorStoryScript.EditorBlocks[blockIndex];
            if (entryIndex < 0 || entryIndex >= block.EditorEntries.Count)
                return false;

            var entry = block.EditorEntries[entryIndex];
            return entry.ValidateEntry(block, entryIndex);
        }
    }
}