using UnityEngine;
using UnityEditor;
using StoryEditor.Controllers;
using StoryEditor.Serialization;

namespace StoryEditor.UI
{
    public class StoryEditorToolbar
    {
        /****** Public Members ******/
        
        public string CurrentFilePath => _currentFilePath;

        public StoryEditorToolbar(EditorStoryScript storyScript, ValidationController validation)
        {
            Debug.Assert(null != storyScript, "Story script cannot be null");
            Debug.Assert(null != validation, "Validation controller cannot be null");

            _editorStoryScript = storyScript;
            _validationController = validation;
        }

        public void SetCallbacks(string filePath, System.Action onNewFile, System.Action onFileLoaded)
        {
            _currentFilePath = filePath;
            _onNewFile = onNewFile;
            _onFileLoaded = onFileLoaded;
        }

        public void Draw()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            DrawFileButtons();
            GUILayout.Space(20);
            GUILayout.FlexibleSpace();
            DrawValidationStatus();

            EditorGUILayout.EndHorizontal();
        }



        /****** Private Members ******/

        private EditorStoryScript _editorStoryScript;
        private ValidationController _validationController;
        private string _currentFilePath;
        private System.Action _onNewFile;
        private System.Action _onFileLoaded;

        private void DrawFileButtons()
        {
            if (GUILayout.Button("New", EditorStyles.toolbarButton))
            {
                NewFile();
            }

            if (GUILayout.Button("Load", EditorStyles.toolbarButton))
            {
                LoadFile();
            }

            if (GUILayout.Button("Save", EditorStyles.toolbarButton))
            {
                SaveFile();
            }

            if (GUILayout.Button("Save As", EditorStyles.toolbarButton))
            {
                SaveAsFile();
            }
        }

        private void DrawValidationStatus()
        {
            var validation = _validationController.ValidateAll();
            
            if (validation.HasErrors)
            {
                GUI.color = Color.red;
                if (GUILayout.Button($"Errors: {validation.Errors.Count}", EditorStyles.toolbarButton))
                {
                    ShowValidationMessages("Errors", validation.Errors);
                }
                GUI.color = Color.white;
            }
            else if (validation.HasWarnings)
            {
                GUI.color = Color.yellow;
                if (GUILayout.Button($"Warnings: {validation.Warnings.Count}", EditorStyles.toolbarButton))
                {
                    ShowValidationMessages("Warnings", validation.Warnings);
                }
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.green;
                GUILayout.Label("Valid", EditorStyles.toolbarButton);
                GUI.color = Color.white;
            }
        }

        private void NewFile()
        {
            if (EditorUtility.DisplayDialog("New File", "Create a new story script? Unsaved changes will be lost.", "New", "Cancel"))
            {
                _currentFilePath = "";
                _onNewFile?.Invoke();
            }
        }

        private void LoadFile()
        {
            var path = EditorUtility.OpenFilePanel("Load Story Script", "", "xml");
            if (false == string.IsNullOrEmpty(path))
            {
                if (StoryScriptSerializer.LoadFromXml(path, out var loadedScript, out var errorMessage))
                {
                    _editorStoryScript.LoadFromStoryScript(loadedScript.ToStoryScript());
                    _currentFilePath = path;
                    _onFileLoaded?.Invoke();
                }
                else
                {
                    EditorUtility.DisplayDialog("Load Error", errorMessage, "OK");
                }
            }
        }

        private void SaveFile()
        {
            UpdateAllChoicesBeforeSave();
            
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                SaveAsFile();
            }
            else
            {
                if (StoryScriptSerializer.SaveToXml(_editorStoryScript, _currentFilePath, out var errorMessage))
                {
                    EditorUtility.DisplayDialog("Save Success", "File saved successfully!", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Save Error", errorMessage, "OK");
                }
            }
        }

        private void SaveAsFile()
        {
            UpdateAllChoicesBeforeSave();
            
            var path = EditorUtility.SaveFilePanel("Save Story Script", "", "StoryScript", "xml");
            if (false == string.IsNullOrEmpty(path))
            {
                if (StoryScriptSerializer.SaveToXml(_editorStoryScript, path, out var errorMessage))
                {
                    _currentFilePath = path;
                    EditorUtility.DisplayDialog("Save Success", "File saved successfully!", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Save Error", errorMessage, "OK");
                }
            }
        }

        private void UpdateAllChoicesBeforeSave()
        {
            for (int blockIndex = 0; blockIndex < _editorStoryScript.EditorBlocks.Count; blockIndex++)
            {
                for (int entryIndex = 0; entryIndex < _editorStoryScript.EditorBlocks[blockIndex].EditorEntries.Count; entryIndex++)
                {
                    var entry = _editorStoryScript.EditorBlocks[blockIndex].EditorEntries[entryIndex];
                    if (entry.IsChoice())
                    {
                        entry.UpdateChoicePrevDialogue(_editorStoryScript.EditorBlocks[blockIndex], entryIndex);
                    }
                }
            }
        }

        private void ShowValidationMessages(string title, System.Collections.Generic.List<string> messages)
        {
            foreach (var message in messages)
            {
                if ("Errors" == title)
                {
                    Logger.Write(LogCategory.StoryScriptEditor, message, LogLevel.Error);
                }
                else
                {
                    Logger.Write(LogCategory.StoryScriptEditor, message, LogLevel.Warning);
                }
            }

            var messageText = string.Join("\n• ", messages);
            EditorUtility.DisplayDialog(title, $"• {messageText}", "OK");
        }
    }
}