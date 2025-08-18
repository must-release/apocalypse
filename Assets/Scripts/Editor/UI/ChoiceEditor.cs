using UnityEngine;
using UnityEditor;
using AD.Story;
using StoryEditor.Controllers;
using System.Collections.Generic;

namespace StoryEditor.UI
{
    public class ChoiceEditor : IStoryEntryEditor
    {
        /****** Public Members ******/

        public ChoiceEditor(ValidationController validationController, EditorStoryScript editorStoryScript)
        {
            Debug.Assert(null != validationController, "Validation controller cannot be null");
            Debug.Assert(null != editorStoryScript, "Editor story script cannot be null");
            
            _validationController = validationController;
            _editorStoryScript = editorStoryScript;
        }

        public void Draw(EditorStoryEntry entry)
        {
            var choice = entry.AsChoice();
            if (null == choice) return;

            DrawPreviousDialogue(choice);
            EditorGUILayout.Space();
            DrawChoiceOptions(choice);
        }


        /****** Private Members ******/

        private ValidationController _validationController;
        private EditorStoryScript _editorStoryScript;

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

            DrawBranchField(option);
            DrawOptionText(option, index);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DrawBranchField(StoryChoiceOption option)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Branch Name:", GUILayout.Width(100));
            var availableBranches = _validationController.GetAvailableBranchNames(_editorStoryScript.SelectedBlockIndex);
            availableBranches.Insert(0, StoryBlock.CommonBranch);
            
            var branchIndex = availableBranches.IndexOf(option.BranchName ?? StoryBlock.CommonBranch);
            if (branchIndex == -1) branchIndex = 0;
            
            var newBranchIndex = EditorGUILayout.Popup(branchIndex, availableBranches.ToArray(), GUILayout.Width(100));
            if (branchIndex != newBranchIndex && 0 <= newBranchIndex && newBranchIndex < availableBranches.Count)
            {
                option.BranchName = availableBranches[newBranchIndex];
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawOptionText(StoryChoiceOption option, int index)
        {
            EditorGUILayout.LabelField("Option Text:");
            var selectedEntry = _editorStoryScript.SelectedEntry;
            var choice = selectedEntry.AsChoice();
            var optionControlName = $"ChoiceOption_{choice.GetHashCode()}_{index}";
            GUI.SetNextControlName(optionControlName);
            option.Text = EditorGUILayout.TextField(option.Text ?? "");
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