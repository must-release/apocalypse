using UnityEngine;
using AD.Story;

namespace StoryEditor
{
    [System.Serializable]
    public class EditorStoryEntry
    {

        /****** Public Members ******/

        public StoryEntry StoryEntry
        {
            get => _storyEntry;
            set => _storyEntry = value;
        }

        public EditorStoryEntry()
        {
            _storyEntry = null;
        }

        public EditorStoryEntry(StoryEntry entry)
        {
            _storyEntry = entry;
        }

        public string GetEntryType()
        {
            if (null == _storyEntry) return "Unknown";

            return _storyEntry.GetType().Name switch
            {
                nameof(StoryDialogue) => "Dialogue",
                nameof(StoryVFX) => "VFX",
                nameof(StoryChoice) => "Choice",
                nameof(StoryCharacterCG) => "CharacterCG",
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
            if (null == _storyEntry) return "Empty Entry";

            return _storyEntry switch
            {
                StoryDialogue dialogue => $"Dialogue: {dialogue.Name} - {TruncateText(dialogue.Text, 12)}",
                StoryVFX vfx => $"VFX: {vfx.VFX} ({vfx.Duration}s)",
                StoryChoice choice => $"Choice: [{GetChoiceOptionsText(choice)}]",
                StoryCharacterCG characterCG => $"CharacterCG: {characterCG.Name} ({characterCG.Animation}, {characterCG.TargetPosition})",
                StoryPlayMode playMode => $"PlayMode: {playMode.PlayMode}",
                StoryBackgroundCG backgroundCG => $"BackgroundCG: {backgroundCG.Chapter} - {(string.IsNullOrEmpty(backgroundCG.ImageName) ? "No Image" : backgroundCG.ImageName)}",
                StoryBGM bgm => $"BGM {bgm.Action}: {(StoryBGM.BGMAction.Start == bgm.Action ? (string.IsNullOrEmpty(bgm.BGMName) ? "No BGM" : bgm.BGMName) : "")} (Fade: {bgm.FadeDuration}s{(StoryBGM.BGMAction.Start == bgm.Action ? $", Loop: {bgm.IsLoop}" : "")})",
                StorySFX sfx => $"SFX: {(string.IsNullOrEmpty(sfx.SFXName) ? "No SFX" : sfx.SFXName)}",
                StoryCameraAction cameraAction => $"Camera: {cameraAction.ActionType} - {(string.IsNullOrEmpty(cameraAction.CameraName) ? "No Camera" : cameraAction.CameraName)} ({cameraAction.Duration}s)",
                _ => "Unknown Entry"
            };
        }

        public bool IsDialogue() => _storyEntry is StoryDialogue;
        public bool IsVFX() => _storyEntry is StoryVFX;
        public bool IsChoice() => _storyEntry is StoryChoice;
        public bool IsCharacterCG() => _storyEntry is StoryCharacterCG;
        public bool IsPlayMode() => _storyEntry is StoryPlayMode;
        public bool IsBackgroundCG() => _storyEntry is StoryBackgroundCG;
        public bool IsBGM() => _storyEntry is StoryBGM;
        public bool IsSFX() => _storyEntry is StorySFX;
        public bool IsCameraAction() => _storyEntry is StoryCameraAction;

        public StoryDialogue AsDialogue() => _storyEntry as StoryDialogue;
        public StoryVFX AsVFX() => _storyEntry as StoryVFX;
        public StoryChoice AsChoice() => _storyEntry as StoryChoice;
        public StoryCharacterCG AsCharacterCG() => _storyEntry as StoryCharacterCG;
        public StoryPlayMode AsPlayMode() => _storyEntry as StoryPlayMode;
        public StoryBackgroundCG AsBackgroundCG() => _storyEntry as StoryBackgroundCG;
        public StoryBGM AsBGM() => _storyEntry as StoryBGM;
        public StorySFX AsSFX() => _storyEntry as StorySFX;
        public StoryCameraAction AsCameraAction() => _storyEntry as StoryCameraAction;

        public void UpdateChoicePrevDialogue(EditorStoryBlock parentBlock, int entryIndex)
        {
            Debug.Assert(null != parentBlock, "Parent block cannot be null");
            Debug.Assert(0 <= entryIndex && entryIndex < parentBlock.EditorEntries.Count, "Entry index out of range");
            Debug.Assert(IsChoice(), "Entry must be a choice type");

            var choice = AsChoice();
            StoryDialogue prevDialogue = null;

            // Find the previous dialogue in the same block
            for (int i = entryIndex - 1; 0 <= i; i--)
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

            if (null == _storyEntry) return false;

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
                    Logger.Write(LogCategory.StoryScriptEditor, $"Choice entry at index {entryIndex} has no dialogue before it", LogLevel.Warning);
                    return false;
                }

                // Validate choice options
                if (null == choice.Options || 0 == choice.Options.Count)
                {
                    Logger.Write(LogCategory.StoryScriptEditor, $"Choice entry at index {entryIndex} has no options", LogLevel.Warning);
                    return false;
                }
            }

            return true;
        }


        /****** Private Members ******/

        [SerializeField] private StoryEntry _storyEntry;

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            if (maxLength >= text.Length)
                return text;

            return text.Substring(0, maxLength) + "...";
        }

        private string GetChoiceOptionsText(StoryChoice choice)
        {
            Debug.Assert(null != choice);

            if (null == choice.Options || 0 == choice.Options.Count)
                return "No options";

            var optionTexts = new string[choice.Options.Count];
            for (int i = 0; i < choice.Options.Count; i++)
            {
                optionTexts[i] = choice.Options[i].BranchName ?? "undefined";
            }
            return string.Join(", ", optionTexts);
        }
    }
}