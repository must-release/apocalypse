using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StoryEditor.Controllers
{
    public class ValidationResult
    {
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

        public bool HasErrors => Errors.Count > 0;
        public bool HasWarnings => Warnings.Count > 0;
        public bool HasIssues => HasErrors || HasWarnings;
    }

    public class ValidationController
    {
        private EditorStoryScript editorStoryScript;

        public ValidationController(EditorStoryScript storyScript)
        {
            editorStoryScript = storyScript;
        }

        public ValidationResult ValidateAll()
        {
            var result = new ValidationResult { IsValid = true };

            // Validate basic structure
            ValidateBasicStructure(result);

            // Validate each block
            for (int blockIndex = 0; blockIndex < editorStoryScript.EditorBlocks.Count; blockIndex++)
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

        private void ValidateBasicStructure(ValidationResult result)
        {
            if (editorStoryScript.EditorBlocks.Count == 0)
            {
                result.AddWarning("Story script contains no blocks");
                return;
            }

            // Check for duplicate branch IDs
            var branchIds = new HashSet<string>();
            for (int i = 0; i < editorStoryScript.EditorBlocks.Count; i++)
            {
                var branchId = editorStoryScript.EditorBlocks[i].BranchId;

                if (string.IsNullOrWhiteSpace(branchId))
                {
                    result.AddError($"Block {i + 1} has empty or null BranchId");
                    continue;
                }

                if (branchIds.Contains(branchId))
                {
                    result.AddError($"Duplicate BranchId '{branchId}' found in block {i + 1}");
                }
                else
                {
                    branchIds.Add(branchId);
                }
            }
        }

        private void ValidateBlock(int blockIndex, ValidationResult result)
        {
            if (blockIndex < 0 || blockIndex >= editorStoryScript.EditorBlocks.Count)
            {
                result.AddError($"Invalid block index: {blockIndex}");
                return;
            }

            var block = editorStoryScript.EditorBlocks[blockIndex];
            var blockName = $"Block {blockIndex + 1} ({block.BranchId})";

            if (block.EditorEntries.Count == 0)
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
            var block = editorStoryScript.EditorBlocks[blockIndex];
            var entry = block.EditorEntries[entryIndex];
            var entryName = $"Block {blockIndex + 1} ({block.BranchId}), Entry {entryIndex + 1}";

            if (entry.StoryEntry == null)
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

                case StoryCharacterStanding character:
                    ValidateCharacterStanding(character, entryName, result);
                    break;

                default:
                    result.AddWarning($"{entryName}: Unknown entry type {entry.StoryEntry.GetType().Name}");
                    break;
            }
        }

        private void ValidateDialogue(StoryDialogue dialogue, string entryName, ValidationResult result)
        {
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
            if (string.IsNullOrWhiteSpace(vfx.Action))
            {
                result.AddError($"{entryName}: VFX has no action specified");
            }

            if (vfx.Duration < 0)
            {
                result.AddError($"{entryName}: VFX duration cannot be negative");
            }
        }

        private void ValidateChoice(StoryChoice choice, int blockIndex, int entryIndex, string entryName, ValidationResult result)
        {
            // Check if there's a dialogue before this choice
            bool hasDialogueBefore = false;
            var block = editorStoryScript.EditorBlocks[blockIndex];

            for (int i = entryIndex - 1; i >= 0; i--)
            {
                if (block.EditorEntries[i].IsDialogue())
                {
                    hasDialogueBefore = true;
                    break;
                }
            }

            if (!hasDialogueBefore)
            {
                result.AddError($"{entryName}: Choice has no dialogue before it");
            }

            // Validate choice options
            if (choice.Options == null || choice.Options.Count == 0)
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

                if (string.IsNullOrWhiteSpace(option.BranchId))
                {
                    result.AddError($"{optionName}: Option has no BranchId");
                }
            }
        }

        private void ValidateCharacterStanding(StoryCharacterStanding character, string entryName, ValidationResult result)
        {
            // Character standing validation would depend on the actual implementation
            // This is a placeholder for now
        }

        private void ValidateChoiceReferences(ValidationResult result)
        {
            for (int blockIndex = 0; blockIndex < editorStoryScript.EditorBlocks.Count; blockIndex++)
            {
                var block = editorStoryScript.EditorBlocks[blockIndex];
                var availableBranches = editorStoryScript.GetAvailableBranchIds(blockIndex);

                for (int entryIndex = 0; entryIndex < block.EditorEntries.Count; entryIndex++)
                {
                    var entry = block.EditorEntries[entryIndex];
                    if (entry.IsChoice())
                    {
                        var choice = entry.AsChoice();
                        var entryName = $"Block {blockIndex + 1} ({block.BranchId}), Entry {entryIndex + 1}";

                        if (choice.Options != null)
                        {
                            for (int optionIndex = 0; optionIndex < choice.Options.Count; optionIndex++)
                            {
                                var option = choice.Options[optionIndex];
                                var optionName = $"{entryName}, Option {optionIndex + 1}";

                                if (!string.IsNullOrWhiteSpace(option.BranchId) && 
                                    !option.BranchId.Equals("common", System.StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!availableBranches.Contains(option.BranchId))
                                    {
                                        result.AddError($"{optionName}: BranchId '{option.BranchId}' does not exist in subsequent blocks");
                                    }
                                }
                            }
                        }
                    }
                }
            }
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

        public List<string> GetAvailableBranchIds(int afterBlockIndex)
        {
            return editorStoryScript.GetAvailableBranchIds(afterBlockIndex);
        }

        public bool IsBranchIdValid(string branchId, int excludeBlockIndex = -1)
        {
            if (string.IsNullOrWhiteSpace(branchId))
                return false;

            for (int i = 0; i < editorStoryScript.EditorBlocks.Count; i++)
            {
                if (i != excludeBlockIndex && 
                    editorStoryScript.EditorBlocks[i].BranchId.Equals(branchId, System.StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }
}