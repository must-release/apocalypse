using System.Collections.Generic;
using UnityEngine;
using AD.Story;

namespace StoryEditor.Controllers
{
    public class EntryController
    {

        /****** Public Members ******/

        public EntryController(EditorStoryScript storyScript)
        {
            Debug.Assert(null != storyScript);

            _editorStoryScript = storyScript;
        }

        public void SetCallbacks(System.Action onEntriesChanged, System.Action onSelectionChanged)
        {
            this._onEntriesChanged = onEntriesChanged;
            this._onSelectionChanged = onSelectionChanged;
        }

        public EditorStoryEntry AddEntry(StoryEntry.EntryType entryType, string character = "독백", string text = "", string action = "", float duration = 0f)
        {
            var selectedBlock = _editorStoryScript.SelectedBlock;
            if (null == selectedBlock)
            {
                Logger.Write(LogCategory.StoryScriptEditor, "No block selected. Cannot add entry.", LogLevel.Warning);
                return null;
            }

            EditorStoryEntry newEntry = null;

            switch (entryType)
            {
                case StoryEntry.EntryType.Dialogue:
                    newEntry = selectedBlock.AddDialogue(character, text);
                    break;
                case StoryEntry.EntryType.VFX:
                    newEntry = selectedBlock.AddVFX(action, duration);
                    break;
                case StoryEntry.EntryType.Choice:
                    newEntry = selectedBlock.AddChoice();
                    break;
                case StoryEntry.EntryType.CharacterCG:
                    newEntry = selectedBlock.AddCharacterCG();
                    break;
                case StoryEntry.EntryType.PlayMode:
                    newEntry = selectedBlock.AddPlayMode();
                    break;
                case StoryEntry.EntryType.BackgroundCG:
                    newEntry = selectedBlock.AddBackgroundCG();
                    break;
                case StoryEntry.EntryType.BGM:
                    newEntry = selectedBlock.AddBGM();
                    break;
                case StoryEntry.EntryType.SFX:
                    newEntry = selectedBlock.AddSFX();
                    break;
                case StoryEntry.EntryType.CameraAction:
                    newEntry = selectedBlock.AddCameraAction();
                    break;
            }

            if (null != newEntry)
            {
                // Select the new entry
                _editorStoryScript.SelectedEntryIndex = selectedBlock.EditorEntries.Count - 1;
                selectedBlock.SelectedEntryIndex = _editorStoryScript.SelectedEntryIndex;

                _onEntriesChanged?.Invoke();
                _onSelectionChanged?.Invoke();
            }

            return newEntry;
        }

        public bool RemoveEntry(int blockIndex, int entryIndex)
        {
            Debug.Assert(0 <= blockIndex && blockIndex < _editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (0 > blockIndex || _editorStoryScript.EditorBlocks.Count <= blockIndex)
                return false;

            var block = _editorStoryScript.EditorBlocks[blockIndex];
            Debug.Assert(0 <= entryIndex && entryIndex < block.EditorEntries.Count, "Entry index out of range");
            if (0 > entryIndex || block.EditorEntries.Count <= entryIndex)
                return false;
            var result = block.RemoveEntry(entryIndex);

            if (result)
            {
                // Update selected entry index if needed
                if (blockIndex == _editorStoryScript.SelectedBlockIndex)
                {
                    if (entryIndex == _editorStoryScript.SelectedEntryIndex)
                    {
                        _editorStoryScript.SelectedEntryIndex = -1;
                    }
                    else if (entryIndex < _editorStoryScript.SelectedEntryIndex)
                    {
                        _editorStoryScript.SelectedEntryIndex--;
                    }
                }

                _onEntriesChanged?.Invoke();
                _onSelectionChanged?.Invoke();
            }

            return result;
        }

        public bool RemoveSelectedEntry()
        {
            if (0 <= _editorStoryScript.SelectedBlockIndex && 0 <= _editorStoryScript.SelectedEntryIndex)
            {
                return RemoveEntry(_editorStoryScript.SelectedBlockIndex, _editorStoryScript.SelectedEntryIndex);
            }
            return false;
        }

        public void SelectEntry(int entryIndex)
        {
            var selectedBlock = _editorStoryScript.SelectedBlock;
            if (null == selectedBlock) return;

            if (-1 <= entryIndex && entryIndex < selectedBlock.EditorEntries.Count)
            {
                _editorStoryScript.SelectedEntryIndex = entryIndex;
                selectedBlock.SelectedEntryIndex = entryIndex;

                _onSelectionChanged?.Invoke();
            }
        }

        public bool MoveEntry(int blockIndex, int fromIndex, int toIndex)
        {
            Debug.Assert(0 <= blockIndex && blockIndex < _editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (0 > blockIndex || _editorStoryScript.EditorBlocks.Count <= blockIndex)
                return false;

            var block = _editorStoryScript.EditorBlocks[blockIndex];
            Debug.Assert(0 <= fromIndex && fromIndex < block.EditorEntries.Count, "From index out of range");
            Debug.Assert(0 <= toIndex && toIndex < block.EditorEntries.Count, "To index out of range");
            if (0 > fromIndex || block.EditorEntries.Count <= fromIndex || 0 > toIndex || block.EditorEntries.Count <= toIndex)
                return false;
            var result = block.MoveEntry(fromIndex, toIndex);

            if (result)
            {
                // Update selected entry index if needed
                if (blockIndex == _editorStoryScript.SelectedBlockIndex)
                {
                    if (fromIndex == _editorStoryScript.SelectedEntryIndex)
                    {
                        _editorStoryScript.SelectedEntryIndex = toIndex;
                    }
                    else if (fromIndex < _editorStoryScript.SelectedEntryIndex && 
                             toIndex >= _editorStoryScript.SelectedEntryIndex)
                    {
                        _editorStoryScript.SelectedEntryIndex--;
                    }
                    else if (fromIndex > _editorStoryScript.SelectedEntryIndex && 
                             toIndex <= _editorStoryScript.SelectedEntryIndex)
                    {
                        _editorStoryScript.SelectedEntryIndex++;
                    }
                }

                _onEntriesChanged?.Invoke();
                _onSelectionChanged?.Invoke();
            }

            return result;
        }

        public bool CanMoveEntryUp(int blockIndex, int entryIndex)
        {
            if (0 > blockIndex || _editorStoryScript.EditorBlocks.Count <= blockIndex)
                return false;

            var block = _editorStoryScript.EditorBlocks[blockIndex];
            return 0 < entryIndex && entryIndex < block.EditorEntries.Count;
        }

        public bool CanMoveEntryDown(int blockIndex, int entryIndex)
        {
            if (0 > blockIndex || _editorStoryScript.EditorBlocks.Count <= blockIndex)
                return false;

            var block = _editorStoryScript.EditorBlocks[blockIndex];
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
            if (0 <= _editorStoryScript.SelectedBlockIndex && 0 <= _editorStoryScript.SelectedEntryIndex)
            {
                return MoveEntryUp(_editorStoryScript.SelectedBlockIndex, _editorStoryScript.SelectedEntryIndex);
            }
            return false;
        }

        public bool MoveSelectedEntryDown()
        {
            if (0 <= _editorStoryScript.SelectedBlockIndex && 0 <= _editorStoryScript.SelectedEntryIndex)
            {
                return MoveEntryDown(_editorStoryScript.SelectedBlockIndex, _editorStoryScript.SelectedEntryIndex);
            }
            return false;
        }

        public void UpdateChoiceEntry(EditorStoryEntry choiceEntry, int blockIndex, int entryIndex)
        {
            if (null != choiceEntry && choiceEntry.IsChoice())
            {
                var block = _editorStoryScript.EditorBlocks[blockIndex];
                choiceEntry.UpdateChoicePrevDialogue(block, entryIndex);
                _onEntriesChanged?.Invoke();
            }
        }

        public void UpdateAllChoiceEntriesInBlock(int blockIndex)
        {
            Debug.Assert(0 <= blockIndex && blockIndex < _editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (0 > blockIndex || _editorStoryScript.EditorBlocks.Count <= blockIndex)
                return;

            var block = _editorStoryScript.EditorBlocks[blockIndex];
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
                _onEntriesChanged?.Invoke();
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

            _onEntriesChanged?.Invoke();
        }

        public void RemoveChoiceOption(EditorStoryEntry choiceEntry, int optionIndex)
        {
            Debug.Assert(null != choiceEntry, "Choice entry cannot be null");
            Debug.Assert(choiceEntry.IsChoice(), "Entry must be a choice type");
            
            var choice = choiceEntry.AsChoice();
            Debug.Assert(null != choice.Options, "Choice options cannot be null");
            Debug.Assert(0 <= optionIndex && optionIndex < choice.Options.Count, "Option index out of range");
            
            choice.Options.RemoveAt(optionIndex);
            _onEntriesChanged?.Invoke();
        }

        public EditorStoryEntry GetSelectedEntry()
        {
            return _editorStoryScript.SelectedEntry;
        }

        public int GetSelectedEntryIndex()
        {
            return _editorStoryScript.SelectedEntryIndex;
        }

        public List<EditorStoryEntry> GetEntriesForSelectedBlock()
        {
            return _editorStoryScript.SelectedBlock?.EditorEntries ?? new List<EditorStoryEntry>();
        }

        public bool ValidateEntry(int blockIndex, int entryIndex)
        {
            Debug.Assert(0 <= blockIndex && blockIndex < _editorStoryScript.EditorBlocks.Count, "Block index out of range");
            if (0 > blockIndex || _editorStoryScript.EditorBlocks.Count <= blockIndex)
                return false;

            var block = _editorStoryScript.EditorBlocks[blockIndex];
            Debug.Assert(0 <= entryIndex && entryIndex < block.EditorEntries.Count, "Entry index out of range");
            if (0 > entryIndex || block.EditorEntries.Count <= entryIndex)
                return false;

            var entry = block.EditorEntries[entryIndex];
            return entry.ValidateEntry(block, entryIndex);
        }


        /****** Private Members ******/

        private EditorStoryScript _editorStoryScript;
        private System.Action _onEntriesChanged;
        private System.Action _onSelectionChanged;
    }
}