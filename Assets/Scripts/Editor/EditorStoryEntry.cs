using UnityEngine;

namespace StoryEditor
{
    [System.Serializable]
    public class EditorStoryEntry
    {
        [SerializeField] private StoryEntry storyEntry;

        public StoryEntry StoryEntry 
        { 
            get => storyEntry;
            set => storyEntry = value;
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
            if (null == storyEntry) return "Unknown";

            return storyEntry.GetType().Name switch
            {
                nameof(StoryDialogue) => "Dialogue",
                nameof(StoryVFX) => "VFX",
                nameof(StoryChoice) => "Choice",
                nameof(StoryCharacterStanding) => "CharacterCG",
                nameof(StoryPlayMode) => "PlayMode",
                nameof(StoryBackgroundCG) => "BackgroundCG",
                nameof(StoryBGM) => "BGM",
                nameof(StorySFX) => "SFX",
                nameof(StoryCameraAction) => "CameraAction",
                _ => "Unknown"
            };
        }

        public string GetDisplayText()
        {
            if (null == storyEntry) return "Empty Entry";

            return storyEntry switch
            {
                StoryDialogue dialogue => $"Dialogue: {dialogue.Name} - {TruncateText(dialogue.Text, 12)}",
                StoryVFX vfx => $"VFX: {vfx.VFX} ({vfx.Duration}s)",
                StoryChoice choice => $"Choice: [{GetChoiceOptionsText(choice)}]",
                StoryCharacterStanding characterCG => $"CharacterCG: {characterCG.Name} ({characterCG.Animation}, {characterCG.TargetPosition})",
                StoryPlayMode playMode => $"PlayMode: {playMode.PlayMode}",
                StoryBackgroundCG backgroundCG => $"BackgroundCG: {backgroundCG.Chapter} - {(string.IsNullOrEmpty(backgroundCG.ImageName) ? "No Image" : backgroundCG.ImageName)}",
                StoryBGM bgm => $"BGM {bgm.Action}: {(bgm.Action == StoryBGM.BGMAction.Start ? (string.IsNullOrEmpty(bgm.BGMName) ? "No BGM" : bgm.BGMName) : "")} (Fade: {bgm.FadeDuration}s{(bgm.Action == StoryBGM.BGMAction.Start ? $", Loop: {bgm.IsLoop}" : "")})",
                StorySFX sfx => $"SFX: {(string.IsNullOrEmpty(sfx.SFXName) ? "No SFX" : sfx.SFXName)}",
                StoryCameraAction cameraAction => $"Camera: {cameraAction.ActionType} - {(string.IsNullOrEmpty(cameraAction.CameraName) ? "No Camera" : cameraAction.CameraName)} ({cameraAction.Duration}s)",
                _ => "Unknown Entry"
            };
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            
            if (text.Length <= maxLength)
                return text;
            
            return text.Substring(0, maxLength) + "...";
        }

        private string GetChoiceOptionsText(StoryChoice choice)
        {
            if (null == choice.Options || 0 == choice.Options.Count)
                return "No options";

            var optionTexts = new string[choice.Options.Count];
            for (int i = 0; i < choice.Options.Count; i++)
            {
                optionTexts[i] = choice.Options[i].BranchName ?? "undefined";
            }
            return string.Join(", ", optionTexts);
        }

        public bool IsDialogue() => storyEntry is StoryDialogue;
        public bool IsVFX() => storyEntry is StoryVFX;
        public bool IsChoice() => storyEntry is StoryChoice;
        public bool IsCharacterStanding() => storyEntry is StoryCharacterStanding;
        public bool IsPlayMode() => storyEntry is StoryPlayMode;
        public bool IsBackgroundCG() => storyEntry is StoryBackgroundCG;
        public bool IsBGM() => storyEntry is StoryBGM;
        public bool IsSFX() => storyEntry is StorySFX;
        public bool IsCameraAction() => storyEntry is StoryCameraAction;

        public StoryDialogue AsDialogue() => storyEntry as StoryDialogue;
        public StoryVFX AsVFX() => storyEntry as StoryVFX;
        public StoryChoice AsChoice() => storyEntry as StoryChoice;
        public StoryCharacterStanding AsCharacterStanding() => storyEntry as StoryCharacterStanding;
        public StoryPlayMode AsPlayMode() => storyEntry as StoryPlayMode;
        public StoryBackgroundCG AsBackgroundCG() => storyEntry as StoryBackgroundCG;
        public StoryBGM AsBGM() => storyEntry as StoryBGM;
        public StorySFX AsSFX() => storyEntry as StorySFX;
        public StoryCameraAction AsCameraAction() => storyEntry as StoryCameraAction;

        public void UpdateChoicePrevDialogue(EditorStoryBlock parentBlock, int entryIndex)
        {
            Debug.Assert(null != parentBlock, "Parent block cannot be null");
            Debug.Assert(0 <= entryIndex && entryIndex < parentBlock.EditorEntries.Count, "Entry index out of range");
            Debug.Assert(IsChoice(), "Entry must be a choice type");

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

            if (null == prevDialogue)
            {
                prevDialogue = new StoryDialogue("", "");
            }

            choice.PrevDialogue = prevDialogue;
        }

        public bool ValidateEntry(EditorStoryBlock parentBlock, int entryIndex)
        {
            Debug.Assert(null != parentBlock, "Parent block cannot be null");
            Debug.Assert(0 <= entryIndex && entryIndex < parentBlock.EditorEntries.Count, "Entry index out of range");
            
            if (null == storyEntry) return false;

            if (IsChoice())
            {
                var choice = AsChoice();
                
                // Check if there's a dialogue before this choice
                bool hasDialogueBefore = false;
                for (int i = entryIndex - 1; 0 <= i; i--)
                {
                    if (parentBlock.EditorEntries[i].IsDialogue())
                    {
                        hasDialogueBefore = true;
                        break;
                    }
                }

                if (false == hasDialogueBefore)
                {
                    Debug.LogWarning($"Choice entry at index {entryIndex} has no dialogue before it");
                    return false;
                }

                // Validate choice options
                if (null == choice.Options || 0 == choice.Options.Count)
                {
                    Debug.LogWarning($"Choice entry at index {entryIndex} has no options");
                    return false;
                }
            }

            return true;
        }
    }
}