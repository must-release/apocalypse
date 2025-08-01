using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace StoryEditor.Serialization
{
    public static class StoryScriptSerializer
    {
        public static bool SaveToXml(EditorStoryScript editorStoryScript, string filePath, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                // Convert to Unity StoryScript format
                var storyScript = editorStoryScript.ToStoryScript();

                // Validate before saving
                var validationController = new Controllers.ValidationController(editorStoryScript);
                if (!validationController.CanSaveStoryScript(out errorMessage))
                {
                    return false;
                }

                // Update choice prevDialogue entries before saving
                UpdateChoicePrevDialogues(editorStoryScript);

                // Create XML serializer for StoryScript
                var serializer = new XmlSerializer(typeof(StoryScript));
                
                // Create XML writer settings for pretty formatting
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\n",
                    NewLineHandling = NewLineHandling.Replace,
                    OmitXmlDeclaration = false,
                    Encoding = System.Text.Encoding.UTF8
                };

                // Write to file
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var xmlWriter = XmlWriter.Create(fileStream, settings))
                {
                    serializer.Serialize(xmlWriter, storyScript);
                }

                Debug.Log($"Story script saved successfully to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to save XML file: {ex.Message}";
                Debug.LogError(errorMessage);
                return false;
            }
        }

        public static bool LoadFromXml(string filePath, out EditorStoryScript editorStoryScript, out string errorMessage)
        {
            editorStoryScript = null;
            errorMessage = "";

            try
            {
                if (!File.Exists(filePath))
                {
                    errorMessage = $"File does not exist: {filePath}";
                    return false;
                }

                // Create XML serializer for StoryScript
                var serializer = new XmlSerializer(typeof(StoryScript));

                StoryScript storyScript;
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    storyScript = (StoryScript)serializer.Deserialize(fileStream);
                }

                // Convert to EditorStoryScript
                editorStoryScript = new EditorStoryScript(storyScript);

                Debug.Log($"Story script loaded successfully from: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to load XML file: {ex.Message}";
                Debug.LogError(errorMessage);
                return false;
            }
        }












        private static void UpdateChoicePrevDialogues(EditorStoryScript editorStoryScript)
        {
            foreach (var block in editorStoryScript.EditorBlocks)
            {
                for (int i = 0; i < block.EditorEntries.Count; i++)
                {
                    var entry = block.EditorEntries[i];
                    if (entry.IsChoice())
                    {
                        entry.UpdateChoicePrevDialogue(block, i);
                    }
                }
            }
        }
    }
}