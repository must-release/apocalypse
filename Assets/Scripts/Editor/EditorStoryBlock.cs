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
            var newVFX = new StoryVFX
            {
                VFX = StoryVFX.VFXType.ScreenFadeIn,
                Duration = duration > 0f ? duration : 1.0f
            };
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

            // Create choice with one default option
            var defaultOptions = new List<StoryChoiceOption>
            {
                new StoryChoiceOption
                {
                    BranchName = StoryBlock.CommonBranch,
                    Text = ""
                }
            };
            
            var newChoice = new StoryChoice(prevDialogue, defaultOptions);
            var editorEntry = new EditorStoryEntry(newChoice);
            editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddCharacterStanding(string name = "", string expression = "")
        {
            var newStanding = new StoryCharacterStanding
            {
                Name = name,
                Expression = expression,
                Animation = StoryCharacterStanding.AnimationType.None,
                TargetPosition = StoryCharacterStanding.TargetPositionType.Center,
                AnimationSpeed = 1.0f,
                IsBlockingAnimation = true
            };
            var editorEntry = new EditorStoryEntry(newStanding);
            editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddPlayMode()
        {
            var newPlayMode = new StoryPlayMode
            {
                PlayMode = StoryPlayMode.PlayModeType.VisualNovel
            };
            var editorEntry = new EditorStoryEntry(newPlayMode);
            editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddBackgroundCG()
        {
            var newBackgroundCG = new StoryBackgroundCG
            {
                Chapter = ChapterType.Test,
                ImageName = ""
            };
            var editorEntry = new EditorStoryEntry(newBackgroundCG);
            editorEntries.Add(editorEntry);
            return editorEntry;
        }


        public EditorStoryEntry AddBGM()
        {
            var newBGM = new StoryBGM
            {
                Action = StoryBGM.BGMAction.Start,
                BGMName = "",
                FadeDuration = 1.0f,
                IsLoop = true
            };
            var editorEntry = new EditorStoryEntry(newBGM);
            editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddSFX()
        {
            var newSFX = new StorySFX
            {
                SFXName = ""
            };
            var editorEntry = new EditorStoryEntry(newSFX);
            editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddCameraAction()
        {
            var newCameraAction = new StoryCameraAction
            {
                ActionType = StoryCameraAction.CameraActionType.SwitchToCamera,
                CameraName = "",
                Duration = 1.0f,
                Priority = 10,
                WaitForCompletion = true
            };
            var editorEntry = new EditorStoryEntry(newCameraAction);
            editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public bool RemoveEntry(int index)
        {
            Debug.Assert(0 <= index && index < editorEntries.Count, "Entry index out of range");
            if (index < 0 || editorEntries.Count <= index)
                return false;
            
            // Clear text content before removing to prevent Unity GUI cache issues
            var entry = editorEntries[index];
            if (entry.IsDialogue())
            {
                var dialogue = entry.AsDialogue();
                dialogue.Text = "";
                
                // Also clear the Unity GUI control's cached text
                var controlName = $"DialogueText_{dialogue.GetHashCode()}";
                GUI.FocusControl(controlName);
                GUI.FocusControl(null); // This forces Unity to save the empty state
            }
            else if (entry.IsChoice())
            {
                var choice = entry.AsChoice();
                if (null != choice.Options)
                {
                    for (int i = 0; i < choice.Options.Count; i++)
                    {
                        var option = choice.Options[i];
                        option.Text = "";
                        
                        // Also clear the Unity GUI control's cached text for each option
                        var optionControlName = $"ChoiceOption_{choice.GetHashCode()}_{i}";
                        GUI.FocusControl(optionControlName);
                        GUI.FocusControl(null);
                    }
                }
            }
            
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