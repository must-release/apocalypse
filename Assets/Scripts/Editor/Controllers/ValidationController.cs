using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AD.Story;
using AD.Camera;

namespace StoryEditor.Controllers
{
    public class ValidationResult
    {

        /****** Public Members ******/

        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
            IsValid = false;
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }

        public bool HasErrors => 0 < Errors.Count;
        public bool HasWarnings => 0 < Warnings.Count;
        public bool HasIssues => HasErrors || HasWarnings;
    }

    public class ValidationController
    {

        /****** Public Members ******/

        public ValidationController(EditorStoryScript storyScript)
        {
            Debug.Assert(null != storyScript);

            _editorStoryScript = storyScript;
        }

        public ValidationResult ValidateAll()
        {
            var result = new ValidationResult { IsValid = true };

            // Validate basic structure
            ValidateBasicStructure(result);

            // Validate each block
            for (int blockIndex = 0; blockIndex < _editorStoryScript.EditorBlocks.Count; blockIndex++)
            {
                ValidateBlock(blockIndex, result);
            }

            // Validate choice references
            ValidateChoiceReferences(result);

            return result;
        }

        public ValidationResult ValidateBlock(int blockIndex)
        {
            var result = new ValidationResult { IsValid = true };
            ValidateBlock(blockIndex, result);
            return result;
        }

        public ValidationResult ValidateEntry(int blockIndex, int entryIndex)
        {
            var result = new ValidationResult { IsValid = true };
            ValidateEntry(blockIndex, entryIndex, result);
            return result;
        }

        public bool CanSaveStoryScript(out string errorMessage)
        {
            var validation = ValidateAll();

            if (validation.HasErrors)
            {
                errorMessage = $"Cannot save due to validation errors:\n{string.Join("\n", validation.Errors)}";
                return false;
            }

            errorMessage = "";
            return true;
        }

        public List<string> GetAvailableBranchNames(int afterBlockIndex)
        {
            return _editorStoryScript.GetAvailableBranchNames(afterBlockIndex);
        }

        public bool IsBranchNameValid(string branchName, int excludeBlockIndex = -1)
        {
            if (string.IsNullOrWhiteSpace(branchName))
                return false;

            for (int i = 0; i < _editorStoryScript.EditorBlocks.Count; i++)
            {
                if (excludeBlockIndex != i &&
                    _editorStoryScript.EditorBlocks[i].BranchName.Equals(branchName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }


        /****** Private Members ******/

        private EditorStoryScript _editorStoryScript;

        private void ValidateBasicStructure(ValidationResult result)
        {
            Debug.Assert(null != result);

            if (0 == _editorStoryScript.EditorBlocks.Count)
            {
                result.AddWarning("Story script contains no blocks");
                return;
            }

            // Check for duplicate branch names
            var branchNames = new HashSet<string>();
            for (int i = 0; i < _editorStoryScript.EditorBlocks.Count; i++)
            {
                var branchName = _editorStoryScript.EditorBlocks[i].BranchName;

                if (string.IsNullOrWhiteSpace(branchName))
                {
                    result.AddError($"Block {i + 1} has empty or null BranchName");
                    continue;
                }

                if (false == StoryBlock.IsCommonBranch(branchName) && branchNames.Contains(branchName))
                {
                    result.AddError($"Duplicate BranchName '{branchName}' found in block {i + 1}");
                }
                else
                {
                    branchNames.Add(branchName);
                }
            }
        }

        private void ValidateBlock(int blockIndex, ValidationResult result)
        {
            Debug.Assert(0 <= blockIndex && blockIndex < _editorStoryScript.EditorBlocks.Count, "Block index out of range");
            Debug.Assert(null != result, "ValidationResult cannot be null");

            var block = _editorStoryScript.EditorBlocks[blockIndex];
            var blockName = $"Block {blockIndex + 1} ({block.BranchName})";

            if (0 == block.EditorEntries.Count)
            {
                result.AddWarning($"{blockName} contains no entries");
                return;
            }

            // Validate each entry in the block
            for (int entryIndex = 0; entryIndex < block.EditorEntries.Count; entryIndex++)
            {
                ValidateEntry(blockIndex, entryIndex, result);
            }
        }

        private void ValidateEntry(int blockIndex, int entryIndex, ValidationResult result)
        {
            Debug.Assert(0 <= blockIndex && blockIndex < _editorStoryScript.EditorBlocks.Count, "Block index out of range");
            Debug.Assert(null != result, "ValidationResult cannot be null");
            if (0 > blockIndex || _editorStoryScript.EditorBlocks.Count <= blockIndex)
                return;

            var block = _editorStoryScript.EditorBlocks[blockIndex];
            Debug.Assert(0 <= entryIndex && entryIndex < block.EditorEntries.Count, "Entry index out of range");
            if (0 > entryIndex || block.EditorEntries.Count <= entryIndex)
                return;

            var entry = block.EditorEntries[entryIndex];
            var entryName = $"Block {blockIndex + 1} ({block.BranchName}), Entry {entryIndex + 1}";

            if (null == entry.StoryEntry)
            {
                result.AddError($"{entryName}: Entry is null");
                return;
            }

            switch (entry.StoryEntry)
            {
                case StoryDialogue dialogue:
                    ValidateDialogue(dialogue, entryName, result);
                    break;

                case StoryVFX vfx:
                    ValidateVFX(vfx, entryName, result);
                    break;

                case StoryChoice choice:
                    ValidateChoice(choice, blockIndex, entryIndex, entryName, result);
                    break;

                case StoryPlayMode playMode:
                    ValidatePlayMode(playMode, entryName, result);
                    break;

                case StoryBGM bgm:
                    ValidateBGM(bgm, entryName, result);
                    break;

                case StoryBackgroundCG backgroundCG:
                    ValidateBackgroundCG(backgroundCG, entryName, result);
                    break;

                case StoryCameraAction cameraAction:
                    ValidateCameraAction(cameraAction, entryName, result);
                    break;

                case StoryCharacterCG characterCG:
                    ValidateCharacterCG(characterCG, entryName, result);
                    break;

                case StorySFX sfx:
                    ValidateSFX(sfx, entryName, result);
                    break;

                default:
                    result.AddWarning($"{entryName}: Unknown entry type {entry.StoryEntry.GetType().Name}");
                    break;
            }
        }

        private void ValidateDialogue(StoryDialogue dialogue, string entryName, ValidationResult result)
        {
            Debug.Assert(null != dialogue);
            Debug.Assert(null != result);

            if (string.IsNullOrWhiteSpace(dialogue.Name))
            {
                result.AddWarning($"{entryName}: Dialogue has no character name");
            }

            if (string.IsNullOrWhiteSpace(dialogue.Text))
            {
                result.AddWarning($"{entryName}: Dialogue has no text");
            }
        }

        private void ValidateVFX(StoryVFX vfx, string entryName, ValidationResult result)
        {
            Debug.Assert(null != vfx);
            Debug.Assert(null != result);

            if (0 > vfx.Duration)
            {
                result.AddError($"{entryName}: VFX duration cannot be negative");
            }
        }

        private void ValidateChoice(StoryChoice choice, int blockIndex, int entryIndex, string entryName, ValidationResult result)
        {
            Debug.Assert(null != choice);
            Debug.Assert(null != result);
            Debug.Assert(0 <= blockIndex && blockIndex < _editorStoryScript.EditorBlocks.Count);

            // Check if PrevDialogue is properly set
            if (null == choice.PrevDialogue)
            {
                result.AddError($"{entryName}: Choice has no previous dialogue reference");
            }
            else if (string.IsNullOrWhiteSpace(choice.PrevDialogue.Name) && string.IsNullOrWhiteSpace(choice.PrevDialogue.Text))
            {
                result.AddError($"{entryName}: Choice previous dialogue is empty");
            }

            // Check if there's a dialogue before this choice
            bool hasDialogueBefore = false;
            var block = _editorStoryScript.EditorBlocks[blockIndex];

            for (int i = entryIndex - 1; 0 <= i; i--)
            {
                if (block.EditorEntries[i].IsDialogue())
                {
                    hasDialogueBefore = true;
                    break;
                }
            }

            if (false == hasDialogueBefore)
            {
                result.AddError($"{entryName}: Choice has no dialogue before it");
            }

            // Validate choice options
            if (null == choice.Options || 0 == choice.Options.Count)
            {
                result.AddError($"{entryName}: Choice has no options");
                return;
            }

            for (int optionIndex = 0; optionIndex < choice.Options.Count; optionIndex++)
            {
                var option = choice.Options[optionIndex];
                var optionName = $"{entryName}, Option {optionIndex + 1}";

                if (string.IsNullOrWhiteSpace(option.Text))
                {
                    result.AddWarning($"{optionName}: Option has no text");
                }

                if (string.IsNullOrWhiteSpace(option.BranchName))
                {
                    result.AddError($"{optionName}: Option has no BranchName");
                }
            }
        }

        private void ValidatePlayMode(StoryPlayMode playMode, string entryName, ValidationResult result)
        {
            Debug.Assert(null != playMode);
            Debug.Assert(null != result);
        }

        private void ValidateBGM(StoryBGM bgm, string entryName, ValidationResult result)
        {
            Debug.Assert(null != bgm);
            Debug.Assert(null != result);

            if (bgm.Action == StoryBGM.BGMAction.Start && string.IsNullOrWhiteSpace(bgm.BGMName))
            {
                result.AddError($"{entryName}: BGM Start action requires BGMName");
            }

            if (0 > bgm.FadeDuration)
            {
                result.AddWarning($"{entryName}: FadeDuration should not be negative");
            }
        }

        private void ValidateBackgroundCG(StoryBackgroundCG backgroundCG, string entryName, ValidationResult result)
        {
            Debug.Assert(null != backgroundCG);
            Debug.Assert(null != result);

            if (string.IsNullOrWhiteSpace(backgroundCG.ImageName))
            {
                result.AddError($"{entryName}: BackgroundCG requires ImageName");
            }
        }

        private void ValidateCameraAction(StoryCameraAction cameraAction, string entryName, ValidationResult result)
        {
            Debug.Assert(null != cameraAction);
            Debug.Assert(null != result);

            switch (cameraAction.ActionType)
            {
                case CameraActionType.SwitchToCamera:
                case CameraActionType.SetPriority:
                    if (string.IsNullOrWhiteSpace(cameraAction.CameraName))
                    {
                        result.AddError($"{entryName}: {cameraAction.ActionType} requires Camera Name");
                    }
                    break;

                case CameraActionType.FollowTarget:
                    if (string.IsNullOrWhiteSpace(cameraAction.TargetName))
                    {
                        result.AddError($"{entryName}: FollowTarget requires TargetName");
                    }
                    break;
            }

            if (0 > cameraAction.Duration)
            {
                result.AddWarning($"{entryName}: Duration should not be negative");
            }
        }

        private void ValidateCharacterCG(StoryCharacterCG characterCG, string entryName, ValidationResult result)
        {
            Debug.Assert(null != characterCG);
            Debug.Assert(null != result);

            if (string.IsNullOrWhiteSpace(characterCG.Name))
            {
                result.AddError($"{entryName}: CharacterCG requires Name");
            }

            if (0 > characterCG.AnimationSpeed)
            {
                result.AddWarning($"{entryName}: AnimationSpeed should not be negative");
            }
        }

        private void ValidateSFX(StorySFX sfx, string entryName, ValidationResult result)
        {
            Debug.Assert(null != sfx);
            Debug.Assert(null != result);

            if (string.IsNullOrWhiteSpace(sfx.SFXName))
            {
                result.AddError($"{entryName}: SFX requires SFXName");
            }
        }


        private void ValidateChoiceReferences(ValidationResult result)
        {
            Debug.Assert(null != result);

            for (int blockIndex = 0; blockIndex < _editorStoryScript.EditorBlocks.Count; blockIndex++)
            {
                var block = _editorStoryScript.EditorBlocks[blockIndex];
                var availableBranches = _editorStoryScript.GetAvailableBranchNames(blockIndex);

                for (int entryIndex = 0; entryIndex < block.EditorEntries.Count; entryIndex++)
                {
                    var entry = block.EditorEntries[entryIndex];
                    if (entry.IsChoice())
                    {
                        var choice = entry.AsChoice();
                        var entryName = $"Block {blockIndex + 1} ({block.BranchName}), Entry {entryIndex + 1}";

                        if (null != choice.Options)
                        {
                            for (int optionIndex = 0; optionIndex < choice.Options.Count; optionIndex++)
                            {
                                var option = choice.Options[optionIndex];
                                var optionName = $"{entryName}, Option {optionIndex + 1}";

                                if (false == string.IsNullOrWhiteSpace(option.BranchName) &&
                                    false == StoryBlock.IsCommonBranch(option.BranchName))
                                {
                                    if (false == availableBranches.Contains(option.BranchName))
                                    {
                                        result.AddError($"{optionName}: BranchName '{option.BranchName}' does not exist in subsequent blocks");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}