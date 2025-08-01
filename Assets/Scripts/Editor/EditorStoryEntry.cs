using UnityEngine;

namespace StoryEditor
{
    [System.Serializable]
    public class EditorStoryEntry
    {
        [SerializeField] private StoryEntry storyEntry;
        [SerializeField] private bool isExpanded = false;

        public StoryEntry StoryEntry 
        { 
            get => storyEntry;
            set => storyEntry = value;
        }

        public bool IsExpanded 
        { 
            get => isExpanded;
            set => isExpanded = value;
        }

        public EditorStoryEntry()
        {
            storyEntry = null;
        }

        public EditorStoryEntry(StoryEntry entry)
        {
            storyEntry = entry;
        }

        public string GetEntryType()
        {
            if (storyEntry == null) return "Unknown";

            return storyEntry.GetType().Name switch
            {
                nameof(StoryDialogue) => "Dialogue",
                nameof(StoryVFX) => "VFX",
                nameof(StoryChoice) => "Choice",
                nameof(StoryCharacterStanding) => "Character",
                _ => "Unknown"
            };
        }

        public string GetDisplayText()
        {
            if (storyEntry == null) return "Empty Entry";

            return storyEntry switch
            {
                StoryDialogue dialogue => $"Dialogue: {dialogue.Name} - {dialogue.Text}",
                StoryVFX vfx => $"VFX: {vfx.Action} ({vfx.Duration}s)",
                StoryChoice choice => $"Choice: [{GetChoiceOptionsText(choice)}]",
                StoryCharacterStanding character => $"Character Standing",
                _ => "Unknown Entry"
            };
        }

        private string GetChoiceOptionsText(StoryChoice choice)
        {
            if (choice.Options == null || choice.Options.Count == 0)
                return "No options";

            var optionTexts = new string[choice.Options.Count];
            for (int i = 0; i < choice.Options.Count; i++)
            {
                optionTexts[i] = choice.Options[i].BranchId ?? "undefined";
            }
            return string.Join(", ", optionTexts);
        }

        public bool IsDialogue() => storyEntry is StoryDialogue;
        public bool IsVFX() => storyEntry is StoryVFX;
        public bool IsChoice() => storyEntry is StoryChoice;
        public bool IsCharacterStanding() => storyEntry is StoryCharacterStanding;

        public StoryDialogue AsDialogue() => storyEntry as StoryDialogue;
        public StoryVFX AsVFX() => storyEntry as StoryVFX;
        public StoryChoice AsChoice() => storyEntry as StoryChoice;
        public StoryCharacterStanding AsCharacterStanding() => storyEntry as StoryCharacterStanding;

        public void UpdateChoicePrevDialogue(EditorStoryBlock parentBlock, int entryIndex)
        {
            if (!IsChoice()) return;

            var choice = AsChoice();
            StoryDialogue prevDialogue = null;

            // Find the previous dialogue in the same block
            for (int i = entryIndex - 1; i >= 0; i--)
            {
                var entry = parentBlock.EditorEntries[i];
                if (entry.IsDialogue())
                {
                    var dialogue = entry.AsDialogue();
                    prevDialogue = new StoryDialogue(dialogue.Name, dialogue.Text);
                    break;
                }
            }

            if (prevDialogue == null)
            {
                prevDialogue = new StoryDialogue("", "");
            }

            choice.PrevDialogue = prevDialogue;
        }

        public bool ValidateEntry(EditorStoryBlock parentBlock, int entryIndex)
        {
            if (storyEntry == null) return false;

            if (IsChoice())
            {
                var choice = AsChoice();
                
                // Check if there's a dialogue before this choice
                bool hasDialogueBefore = false;
                for (int i = entryIndex - 1; i >= 0; i--)
                {
                    if (parentBlock.EditorEntries[i].IsDialogue())
                    {
                        hasDialogueBefore = true;
                        break;
                    }
                }

                if (!hasDialogueBefore)
                {
                    Debug.LogWarning($"Choice entry at index {entryIndex} has no dialogue before it");
                    return false;
                }

                // Validate choice options
                if (choice.Options == null || choice.Options.Count == 0)
                {
                    Debug.LogWarning($"Choice entry at index {entryIndex} has no options");
                    return false;
                }
            }

            return true;
        }
    }
}