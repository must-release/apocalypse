using UnityEngine;
using UnityEditor;
using StoryEditor.Controllers;

namespace StoryEditor.UI
{
    public class StoryEditorPanel
    {
        /****** Public Members ******/

        public StoryEditorPanel(EditorStoryScript storyScript, EntryController entryController, ValidationController validationController)
        {
            Debug.Assert(null != storyScript, "Story script cannot be null");
            Debug.Assert(null != entryController, "Entry controller cannot be null");
            Debug.Assert(null != validationController, "Validation controller cannot be null");
            
            _editorStoryScript = storyScript;
            _entryController = entryController;
            _editorFactory = new StoryEntryEditorFactory(validationController, storyScript);
        }


        /****** Private Members ******/

        private EditorStoryScript _editorStoryScript;
        private EntryController _entryController;
        private StoryEntryEditorFactory _editorFactory;
        private Vector2 _scrollPosition;
        
        // Track selection to force text refresh
        private int _lastSelectedBlockIndex = -1;
        private int _lastSelectedEntryIndex = -1;

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
            var selectedEntry = _editorStoryScript.SelectedEntry;
            if (null == selectedEntry)
            {
                EditorGUILayout.HelpBox("Select an entry to edit its properties", MessageType.Info);
                return;
            }

            // Check if selection changed and notify factory for text refresh
            if (_lastSelectedBlockIndex != _editorStoryScript.SelectedBlockIndex || 
                _lastSelectedEntryIndex != _editorStoryScript.SelectedEntryIndex)
            {
                _lastSelectedBlockIndex = _editorStoryScript.SelectedBlockIndex;
                _lastSelectedEntryIndex = _editorStoryScript.SelectedEntryIndex;
                _editorFactory.NotifyTextRefresh();
                
                // Clear focus to ensure clean transition
                GUI.FocusControl(null);
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Entry Type - " + selectedEntry.GetEntryType(), EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Handle choice entry update before drawing
            if (selectedEntry.StoryEntry is StoryChoice)
            {
                _entryController.UpdateChoiceEntry(selectedEntry, 
                    _editorStoryScript.SelectedBlockIndex, 
                    _editorStoryScript.SelectedEntryIndex);
            }

            // Use factory to get appropriate editor and draw
            var editor = _editorFactory.GetEditor(selectedEntry);
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