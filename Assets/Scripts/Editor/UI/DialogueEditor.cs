using UnityEngine;
using UnityEditor;
using AD.Story;

namespace StoryEditor.UI
{
    public class DialogueEditor : IStoryEntryEditor
    {
        /****** Public Members ******/

        public void Draw(EditorStoryEntry entry)
        {
            var dialogue = entry.AsDialogue();
            if (null == dialogue) return;

            DrawCharacterField(dialogue);
            EditorGUILayout.Space();
            DrawTextSpeedField(dialogue);
            EditorGUILayout.Space();
            DrawAutoSkipField(dialogue);
            EditorGUILayout.Space();
            DrawDialogueText(dialogue);
        }

        public void SetNeedsTextRefresh(bool needsRefresh)
        {
            _needsTextRefresh = needsRefresh;
        }


        /****** Private Members ******/

        private bool _needsTextRefresh = false;

        private void DrawCharacterField(StoryDialogue dialogue)
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
        }

        private void DrawTextSpeedField(StoryDialogue dialogue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Speed:", EditorStyles.boldLabel, GUILayout.Width(80));
            dialogue.TextSpeed = (StoryDialogue.TextSpeedType)EditorGUILayout.EnumPopup(dialogue.TextSpeed, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAutoSkipField(StoryDialogue dialogue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Auto Skip:", EditorStyles.boldLabel, GUILayout.Width(80));
            dialogue.IsAutoSkip = EditorGUILayout.Toggle(dialogue.IsAutoSkip, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDialogueText(StoryDialogue dialogue)
        {
            EditorGUILayout.LabelField("Dialogue Text:", EditorStyles.boldLabel);
            var textStyle = new GUIStyle(EditorStyles.textArea);
            textStyle.wordWrap = true;
            
            var controlName = $"DialogueText_{dialogue.GetHashCode()}";
            GUI.SetNextControlName(controlName);
            
            if (_needsTextRefresh)
            {
                _needsTextRefresh = false;
                GUI.FocusControl(null);
            }
            
            dialogue.Text = EditorGUILayout.TextArea(dialogue.Text ?? "", textStyle, GUILayout.Height(100));
        }
    }
}