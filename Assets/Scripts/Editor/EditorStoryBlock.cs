using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StoryEditor
{
    [System.Serializable]
    public class EditorStoryBlock
    {
        [SerializeField] private string branchId;
        [SerializeField] private List<EditorStoryEntry> editorEntries = new List<EditorStoryEntry>();
        [SerializeField] private int selectedEntryIndex = -1;

        public string BranchId 
        { 
            get => branchId;
            set => branchId = value;
        }

        public List<EditorStoryEntry> EditorEntries => editorEntries;

        public int SelectedEntryIndex 
        { 
            get => selectedEntryIndex;
            set => selectedEntryIndex = value;
        }

        public EditorStoryEntry SelectedEntry => 
            selectedEntryIndex >= 0 && selectedEntryIndex < editorEntries.Count ? 
            editorEntries[selectedEntryIndex] : null;

        public EditorStoryBlock()
        {
            branchId = "";
            editorEntries = new List<EditorStoryEntry>();
        }

        public EditorStoryBlock(string branchId)
        {
            this.branchId = branchId;
            editorEntries = new List<EditorStoryEntry>();
        }

        public EditorStoryBlock(StoryBlock storyBlock)
        {
            LoadFromStoryBlock(storyBlock);
        }

        public void LoadFromStoryBlock(StoryBlock storyBlock)
        {
            branchId = storyBlock.BranchId ?? "";
            editorEntries.Clear();
            selectedEntryIndex = -1;

            if (storyBlock.Entries != null)
            {
                foreach (var entry in storyBlock.Entries)
                {
                    editorEntries.Add(new EditorStoryEntry(entry));
                }
            }
        }

        public StoryBlock ToStoryBlock()
        {
            return new StoryBlock
            {
                BranchId = branchId,
                Entries = editorEntries.Select(ee => ee.StoryEntry).ToList()
            };
        }

        public EditorStoryEntry AddDialogue(string character = "독백", string text = "")
        {
            // Find the last dialogue to use its character as default
            if (string.IsNullOrEmpty(character) || character == "독백")
            {
                var lastDialogue = editorEntries
                    .Where(e => e.StoryEntry is StoryDialogue)
                    .Cast<EditorStoryEntry>()
                    .LastOrDefault();

                if (lastDialogue != null)
                {
                    var dialogue = lastDialogue.StoryEntry as StoryDialogue;
                    if (!string.IsNullOrEmpty(dialogue.Name))
                    {
                        character = dialogue.Name;
                    }
                }
            }

            var newDialogue = new StoryDialogue(character, text);
            var editorEntry = new EditorStoryEntry(newDialogue);
            editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddVFX(string action = "", float duration = 0f)
        {
            var newVFX = new StoryVFX(action, duration);
            var editorEntry = new EditorStoryEntry(newVFX);
            editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddChoice()
        {
            // Find the previous dialogue for the choice
            StoryDialogue prevDialogue = null;
            for (int i = editorEntries.Count - 1; i >= 0; i--)
            {
                if (editorEntries[i].StoryEntry is StoryDialogue dialogue)
                {
                    prevDialogue = new StoryDialogue(dialogue.Name, dialogue.Text);
                    break;
                }
            }

            if (prevDialogue == null)
            {
                prevDialogue = new StoryDialogue("", "");
            }

            var newChoice = new StoryChoice(prevDialogue, new List<StoryChoiceOption>());
            var editorEntry = new EditorStoryEntry(newChoice);
            editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public bool RemoveEntry(int index)
        {
            if (index >= 0 && index < editorEntries.Count)
            {
                editorEntries.RemoveAt(index);
                if (selectedEntryIndex == index)
                {
                    selectedEntryIndex = -1;
                }
                else if (selectedEntryIndex > index)
                {
                    selectedEntryIndex--;
                }
                return true;
            }
            return false;
        }

        public bool MoveEntry(int fromIndex, int toIndex)
        {
            if (fromIndex >= 0 && fromIndex < editorEntries.Count &&
                toIndex >= 0 && toIndex < editorEntries.Count &&
                fromIndex != toIndex)
            {
                var entry = editorEntries[fromIndex];
                editorEntries.RemoveAt(fromIndex);
                editorEntries.Insert(toIndex, entry);

                // Update selected index if needed
                if (selectedEntryIndex == fromIndex)
                {
                    selectedEntryIndex = toIndex;
                }
                else if (selectedEntryIndex > fromIndex && selectedEntryIndex <= toIndex)
                {
                    selectedEntryIndex--;
                }
                else if (selectedEntryIndex < fromIndex && selectedEntryIndex >= toIndex)
                {
                    selectedEntryIndex++;
                }

                return true;
            }
            return false;
        }

        public string GetDisplayName()
        {
            return string.IsNullOrEmpty(branchId) ? "Unnamed Block" : branchId;
        }

        public int GetEntryCount()
        {
            return editorEntries.Count;
        }
    }
}