using UnityEngine;
using UnityEditor;

namespace StoryEditor.UI
{
    public class PlayModeEditor : IStoryEntryEditor
    {
        public void Draw(EditorStoryEntry entry)
        {
            var playMode = entry.AsPlayMode();
            if (null == playMode) return;

            DrawPlayModeField(playMode);
            EditorGUILayout.Space();
            DrawModeDescription(playMode);
        }

        private void DrawPlayModeField(StoryPlayMode playMode)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Play Mode:", EditorStyles.boldLabel, GUILayout.Width(120));
            playMode.PlayMode = (StoryPlayMode.PlayModeType)EditorGUILayout.EnumPopup(playMode.PlayMode, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawModeDescription(StoryPlayMode playMode)
        {
            string description = playMode.PlayMode switch
            {
                StoryPlayMode.PlayModeType.VisualNovel => "Visual Novel mode with full screen dialogue interface.",
                StoryPlayMode.PlayModeType.SideDialogue => "Side dialogue mode with compact UI during gameplay.",
                StoryPlayMode.PlayModeType.InGameCutScene => "In-game cutscene mode with cinematic presentation.",
                _ => "Unknown play mode type."
            };

            EditorGUILayout.HelpBox(description, MessageType.Info);
        }
    }
}