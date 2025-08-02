using UnityEngine;
using UnityEditor;
using StoryEditor.Controllers;
using System.Collections.Generic;

namespace StoryEditor.UI
{
    public class StoryEditorPanel
    {
        private EditorStoryScript editorStoryScript;
        private EntryController entryController;
        private ValidationController validationController;
        private Vector2 scrollPosition;
        
        // Track selection to force text refresh
        private int lastSelectedBlockIndex = -1;
        private int lastSelectedEntryIndex = -1;
        private bool needsTextRefresh = false;
        
        

        public StoryEditorPanel(EditorStoryScript storyScript, EntryController entryController, ValidationController validationController)
        {
            this.editorStoryScript = storyScript;
            this.entryController = entryController;
            this.validationController = validationController;
        }

        public void Draw(Rect rect)
        {
            GUILayout.BeginArea(rect);
            DrawHeader();
            DrawEditor();
            GUILayout.EndArea();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(3);
            GUILayout.Label("Entry Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space(3);
        }

        private void DrawEditor()
        {
            var selectedEntry = editorStoryScript.SelectedEntry;
            if (null == selectedEntry)
            {
                EditorGUILayout.HelpBox("Select an entry to edit its properties", MessageType.Info);
                return;
            }

            // Check if selection changed and mark for text refresh
            if (lastSelectedBlockIndex != editorStoryScript.SelectedBlockIndex || 
                lastSelectedEntryIndex != editorStoryScript.SelectedEntryIndex)
            {
                lastSelectedBlockIndex = editorStoryScript.SelectedBlockIndex;
                lastSelectedEntryIndex = editorStoryScript.SelectedEntryIndex;
                needsTextRefresh = true;
                
                // Clear focus to ensure clean transition
                GUI.FocusControl(null);
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("Entry Type - " + selectedEntry.GetEntryType(), EditorStyles.boldLabel);
            EditorGUILayout.Space();

            switch (selectedEntry.StoryEntry)
            {
                case StoryDialogue dialogue:
                    DrawDialogueEditor(dialogue);
                    break;
                case StoryVFX vfx:
                    DrawVFXEditor(vfx);
                    break;
                case StoryChoice choice:
                    entryController.UpdateChoiceEntry(selectedEntry, 
                        editorStoryScript.SelectedBlockIndex, 
                        editorStoryScript.SelectedEntryIndex);
                    DrawChoiceEditor(choice);
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawDialogueEditor(StoryDialogue dialogue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Character:", EditorStyles.boldLabel, GUILayout.Width(80));
            var characterOptions = new string[] { "독백", "나", "소녀", "중개상" };
            var currentIndex = System.Array.IndexOf(characterOptions, dialogue.Name);
            if (currentIndex == -1) currentIndex = 0;
            
            var newIndex = EditorGUILayout.Popup(currentIndex, characterOptions, GUILayout.Width(80));
            if (newIndex != currentIndex)
            {
                dialogue.Name = characterOptions[newIndex];
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Speed:", EditorStyles.boldLabel, GUILayout.Width(80));
            dialogue.TextSpeed = (StoryDialogue.TextSpeedType)EditorGUILayout.EnumPopup(dialogue.TextSpeed, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Dialogue Text:", EditorStyles.boldLabel);
            var textStyle = new GUIStyle(EditorStyles.textArea);
            textStyle.wordWrap = true;
            
            // Always use a unique control name to prevent Unity from reusing cached text
            // Use dialogue object hash to ensure each dialogue gets its own control
            var controlName = $"DialogueText_{dialogue.GetHashCode()}";
            GUI.SetNextControlName(controlName);
            
            if (needsTextRefresh)
            {
                needsTextRefresh = false;
                // Force focus clear when switching entries
                GUI.FocusControl(null);
            }
            
            dialogue.Text = EditorGUILayout.TextArea(dialogue.Text ?? "", textStyle, GUILayout.Height(100));
        }

        private void DrawVFXEditor(StoryVFX vfx)
        {
            EditorGUILayout.LabelField("Action:", EditorStyles.boldLabel);
            var actionOptions = new string[] { "FadeIn", "FadeOut", "Shake" };
            var currentIndex = System.Array.IndexOf(actionOptions, vfx.Action);
            if (currentIndex == -1) currentIndex = 0;
            
            var newIndex = EditorGUILayout.Popup(currentIndex, actionOptions);
            if (newIndex != currentIndex)
            {
                vfx.Action = actionOptions[newIndex];
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Duration (seconds):", EditorStyles.boldLabel);
            vfx.Duration = EditorGUILayout.FloatField(vfx.Duration);
            if (vfx.Duration < 0) vfx.Duration = 0;
        }

        private void DrawChoiceEditor(StoryChoice choice)
        {
            DrawPreviousDialogue(choice);
            EditorGUILayout.Space();
            DrawChoiceOptions(choice);
        }

        private void DrawPreviousDialogue(StoryChoice choice)
        {
            EditorGUILayout.LabelField("Previous Dialogue:", EditorStyles.boldLabel);
            if (null != choice.PrevDialogue)
            {
                EditorGUILayout.LabelField($"Character: {choice.PrevDialogue.Name}");
                EditorGUILayout.LabelField($"Text: {choice.PrevDialogue.Text}");
            }
            else
            {
                EditorGUILayout.HelpBox("No previous dialogue found", MessageType.Warning);
            }
        }

        private void DrawChoiceOptions(StoryChoice choice)
        {
            EditorGUILayout.LabelField("Choice Options:", EditorStyles.boldLabel);

            if (null == choice.Options)
            {
                choice.Options = new List<StoryChoiceOption>();
            }

            for (int i = 0; i < choice.Options.Count; i++)
            {
                DrawChoiceOption(choice.Options[i], i);
            }

            DrawChoiceButtons(choice);
        }

        private void DrawChoiceOption(StoryChoiceOption option, int index)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            GUILayout.Label($"Option {index + 1}:", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Branch Name:", GUILayout.Width(100));
            var availableBranches = validationController.GetAvailableBranchNames(editorStoryScript.SelectedBlockIndex);
            availableBranches.Insert(0, StoryBlock.CommonBranch);
            
            var branchIndex = availableBranches.IndexOf(option.BranchName ?? StoryBlock.CommonBranch);
            if (branchIndex == -1) branchIndex = 0;
            
            var newBranchIndex = EditorGUILayout.Popup(branchIndex, availableBranches.ToArray(), GUILayout.Width(100));
            if (branchIndex != newBranchIndex && 0 <= newBranchIndex && newBranchIndex < availableBranches.Count)
            {
                option.BranchName = availableBranches[newBranchIndex];
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Option Text:");
            var selectedEntry = editorStoryScript.SelectedEntry;
            var choice = selectedEntry.AsChoice();
            var optionControlName = $"ChoiceOption_{choice.GetHashCode()}_{index}";
            GUI.SetNextControlName(optionControlName);
            option.Text = EditorGUILayout.TextField(option.Text ?? "");

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DrawChoiceButtons(StoryChoice choice)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(30)))
            {
                choice.Options.Add(new StoryChoiceOption
                {
                    BranchName = StoryBlock.CommonBranch,
                    Text = ""
                });
            }
            
            GUI.enabled = choice.Options.Count > 1;
            if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(30)))
            {
                choice.Options.RemoveAt(choice.Options.Count - 1);
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
        }
    }
}