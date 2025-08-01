using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StoryEditor
{
    [System.Serializable]
    public class EditorStoryBlock
    {
        [SerializeField] private string branchName;
        [SerializeField] private List<EditorStoryEntry> editorEntries = new List<EditorStoryEntry>();
        [SerializeField] private int selectedEntryIndex = -1;

        public string BranchName 
        { 
            get => branchName;
            set => branchName = value;
        }

        public List<EditorStoryEntry> EditorEntries => editorEntries;

        public int SelectedEntryIndex 
        { 
            get => selectedEntryIndex;
            set => selectedEntryIndex = value;
        }

        public EditorStoryEntry SelectedEntry => 
            0 <= selectedEntryIndex && selectedEntryIndex < editorEntries.Count ? 
            editorEntries[selectedEntryIndex] : null;

        public EditorStoryBlock()
        {
            branchName = "";
            editorEntries = new List<EditorStoryEntry>();
        }

        public EditorStoryBlock(string branchName)
        {
            this.branchName = branchName;
            editorEntries = new List<EditorStoryEntry>();
        }

        public EditorStoryBlock(StoryBlock storyBlock)
        {
            LoadFromStoryBlock(storyBlock);
        }

        public void LoadFromStoryBlock(StoryBlock storyBlock)
        {
            branchName = storyBlock.BranchName ?? "";
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
                BranchName = branchName,
                Entries = editorEntries.Select(ee => ee.StoryEntry).ToList()
            };
        }

        public EditorStoryEntry AddDialogue(string character = "독백", string text = "")
        {
            // Find the last dialogue to use its character as default
            if (string.IsNullOrEmpty(character) || "독백" == character)
            {
                var lastDialogue = editorEntries
                    .Where(e => e.StoryEntry is StoryDialogue)
                    .Cast<EditorStoryEntry>()
                    .LastOrDefault();

                if (lastDialogue != null)
                {
                    var dialogue = lastDialogue.StoryEntry as StoryDialogue;
                    if (false == string.IsNullOrEmpty(dialogue.Name))
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
            for (int i = editorEntries.Count - 1; 0 <= i; i--)
            {
                if (editorEntries[i].StoryEntry is StoryDialogue dialogue)
                {
                    prevDialogue = new StoryDialogue(dialogue.Name, dialogue.Text);
                    break;
                }
            }

            if (null == prevDialogue)
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
            Debug.Assert(0 <= index && index < editorEntries.Count, "Entry index out of range");
            if (index < 0 || editorEntries.Count <= index)
                return false;
            
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

        public bool MoveEntry(int fromIndex, int toIndex)
        {
            Debug.Assert(0 <= fromIndex && fromIndex < editorEntries.Count, "From index out of range");
            Debug.Assert(0 <= toIndex && toIndex < editorEntries.Count, "To index out of range");
            Debug.Assert(fromIndex != toIndex, "From and to indices cannot be the same");
            if (fromIndex < 0 || editorEntries.Count <= fromIndex || toIndex < 0 || editorEntries.Count <= toIndex || fromIndex == toIndex)
                return false;

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

        public string GetDisplayName()
        {
            return string.IsNullOrEmpty(branchName) ? "Unnamed Block" : branchName;
        }

        public int GetEntryCount()
        {
            return editorEntries.Count;
        }
    }
}