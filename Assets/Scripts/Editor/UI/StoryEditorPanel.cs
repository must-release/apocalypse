using UnityEngine;
using UnityEditor;
using StoryEditor.Controllers;

namespace StoryEditor.UI
{
    public class StoryEditorPanel
    {
        private EditorStoryScript editorStoryScript;
        private EntryController entryController;
        private StoryEntryEditorFactory editorFactory;
        private Vector2 scrollPosition;
        
        // Track selection to force text refresh
        private int lastSelectedBlockIndex = -1;
        private int lastSelectedEntryIndex = -1;

        public StoryEditorPanel(EditorStoryScript storyScript, EntryController entryController, ValidationController validationController)
        {
            this.editorStoryScript = storyScript;
            this.entryController = entryController;
            this.editorFactory = new StoryEntryEditorFactory(validationController, storyScript);
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

            // Check if selection changed and notify factory for text refresh
            if (lastSelectedBlockIndex != editorStoryScript.SelectedBlockIndex || 
                lastSelectedEntryIndex != editorStoryScript.SelectedEntryIndex)
            {
                lastSelectedBlockIndex = editorStoryScript.SelectedBlockIndex;
                lastSelectedEntryIndex = editorStoryScript.SelectedEntryIndex;
                editorFactory.NotifyTextRefresh();
                
                // Clear focus to ensure clean transition
                GUI.FocusControl(null);
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("Entry Type - " + selectedEntry.GetEntryType(), EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Handle choice entry update before drawing
            if (selectedEntry.StoryEntry is StoryChoice)
            {
                entryController.UpdateChoiceEntry(selectedEntry, 
                    editorStoryScript.SelectedBlockIndex, 
                    editorStoryScript.SelectedEntryIndex);
            }

            // Use factory to get appropriate editor and draw
            var editor = editorFactory.GetEditor(selectedEntry);
            if (null != editor)
            {
                editor.Draw(selectedEntry);
            }
            else
            {
                EditorGUILayout.HelpBox($"No editor available for entry type: {selectedEntry.GetEntryType()}", MessageType.Warning);
            }

            EditorGUILayout.EndScrollView();
        }


    }
}