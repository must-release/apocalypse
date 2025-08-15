using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AD.Story;
using AD.Camera;

namespace StoryEditor
{
    [System.Serializable]
    public class EditorStoryBlock
    {

        /****** Public Members ******/

        public string BranchName 
        { 
            get => _branchName;
            set => _branchName = value;
        }

        public List<EditorStoryEntry> EditorEntries => _editorEntries;

        public int SelectedEntryIndex 
        { 
            get => _selectedEntryIndex;
            set => _selectedEntryIndex = value;
        }

        public EditorStoryEntry SelectedEntry => 
            0 <= _selectedEntryIndex && _selectedEntryIndex < _editorEntries.Count ? 
            _editorEntries[_selectedEntryIndex] : null;

        public EditorStoryBlock()
        {
            _branchName = "";
            _editorEntries = new List<EditorStoryEntry>();
        }

        public EditorStoryBlock(string branchName)
        {
            Debug.Assert(null != branchName);

            this._branchName = branchName;
            _editorEntries = new List<EditorStoryEntry>();
        }

        public EditorStoryBlock(StoryBlock storyBlock)
        {
            Debug.Assert(null != storyBlock);

            LoadFromStoryBlock(storyBlock);
        }

        public void LoadFromStoryBlock(StoryBlock storyBlock)
        {
            Debug.Assert(null != storyBlock);

            _branchName = storyBlock.BranchName ?? "";
            _editorEntries.Clear();
            _selectedEntryIndex = -1;

            if (null != storyBlock.Entries)
            {
                foreach (var entry in storyBlock.Entries)
                {
                    _editorEntries.Add(new EditorStoryEntry(entry));
                }
            }
        }

        public StoryBlock ToStoryBlock()
        {
            return new StoryBlock
            {
                BranchName = _branchName,
                Entries = _editorEntries.Select(ee => ee.StoryEntry).ToList()
            };
        }

        public EditorStoryEntry AddDialogue(string character = "독백", string text = "")
        {
            // Find the last dialogue to use its character as default
            if (string.IsNullOrEmpty(character) || "독백" == character)
            {
                var lastDialogue = _editorEntries
                    .Where(e => e.StoryEntry is StoryDialogue)
                    .Cast<EditorStoryEntry>()
                    .LastOrDefault();

                if (null != lastDialogue)
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
            _editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddVFX(string action = "", float duration = 0f)
        {
            var newVFX = new StoryVFX
            {
                VFX = StoryVFX.VFXType.ScreenFadeIn,
                Duration = 0f < duration ? duration : 1.0f
            };
            var editorEntry = new EditorStoryEntry(newVFX);
            _editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddChoice()
        {
            // Find the previous dialogue for the choice
            StoryDialogue prevDialogue = null;
            for (int i = _editorEntries.Count - 1; 0 <= i; i--)
            {
                if (_editorEntries[i].StoryEntry is StoryDialogue dialogue)
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
            _editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddCharacterCG(string name = "나", StoryCharacterCG.FacialExpressionType expression = StoryCharacterCG.FacialExpressionType.Default)
        {
            var newCharacterCG = new StoryCharacterCG
            {
                Name = name,
                Expression = expression,
                Animation = StoryCharacterCG.AnimationType.None,
                TargetPosition = StoryCharacterCG.TargetPositionType.Center,
                AnimationSpeed = 1.0f,
                IsBlockingAnimation = true
            };
            var editorEntry = new EditorStoryEntry(newCharacterCG);
            _editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddPlayMode()
        {
            var newPlayMode = new StoryPlayMode
            {
                PlayMode = StoryPlayMode.PlayModeType.VisualNovel
            };
            var editorEntry = new EditorStoryEntry(newPlayMode);
            _editorEntries.Add(editorEntry);
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
            _editorEntries.Add(editorEntry);
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
            _editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddSFX()
        {
            var newSFX = new StorySFX
            {
                SFXName = ""
            };
            var editorEntry = new EditorStoryEntry(newSFX);
            _editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public EditorStoryEntry AddCameraAction()
        {
            var newCameraAction = new StoryCameraAction
            {
                ActionType = CameraActionType.SwitchToCamera,
                CameraName = "",
                Duration = 1.0f,
                Priority = 10
            };
            var editorEntry = new EditorStoryEntry(newCameraAction);
            _editorEntries.Add(editorEntry);
            return editorEntry;
        }

        public bool RemoveEntry(int index)
        {
            Debug.Assert(0 <= index && index < _editorEntries.Count, "Entry index out of range");
            if (0 > index || _editorEntries.Count <= index)
                return false;
            
            // Clear text content before removing to prevent Unity GUI cache issues
            var entry = _editorEntries[index];
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
            
            _editorEntries.RemoveAt(index);
            if (index == _selectedEntryIndex)
            {
                _selectedEntryIndex = -1;
            }
            else if (index < _selectedEntryIndex)
            {
                _selectedEntryIndex--;
            }
            return true;
        }

        public bool MoveEntry(int fromIndex, int toIndex)
        {
            Debug.Assert(0 <= fromIndex && fromIndex < _editorEntries.Count, "From index out of range");
            Debug.Assert(0 <= toIndex && toIndex < _editorEntries.Count, "To index out of range");
            if (0 > fromIndex || _editorEntries.Count <= fromIndex || 0 > toIndex || _editorEntries.Count <= toIndex || fromIndex == toIndex)
                return false;

            var entry = _editorEntries[fromIndex];
            _editorEntries.RemoveAt(fromIndex);
            _editorEntries.Insert(toIndex, entry);

            // Update selected index if needed
            if (fromIndex == _selectedEntryIndex)
            {
                _selectedEntryIndex = toIndex;
            }
            else if (fromIndex < _selectedEntryIndex && toIndex >= _selectedEntryIndex)
            {
                _selectedEntryIndex--;
            }
            else if (fromIndex > _selectedEntryIndex && toIndex <= _selectedEntryIndex)
            {
                _selectedEntryIndex++;
            }

            return true;
        }

        public string GetDisplayName()
        {
            return string.IsNullOrEmpty(_branchName) ? "Unnamed Block" : _branchName;
        }

        public int GetEntryCount()
        {
            return _editorEntries.Count;
        }


        /****** Private Members ******/

        [SerializeField] private string _branchName;
        [SerializeField] private List<EditorStoryEntry> _editorEntries = new List<EditorStoryEntry>();
        [SerializeField] private int _selectedEntryIndex = -1;
    }
}