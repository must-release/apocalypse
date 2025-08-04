using UnityEngine;
using UnityEditor;

namespace StoryEditor.UI
{
    public class CharacterStandingEditor : IStoryEntryEditor
    {
        public void Draw(EditorStoryEntry entry)
        {
            var standing = entry.AsCharacterStanding();
            if (null == standing) return;

            DrawCharacterNameField(standing);
            EditorGUILayout.Space();
            DrawExpressionField(standing);
            EditorGUILayout.Space();
            DrawAnimationField(standing);
            EditorGUILayout.Space();
            DrawTargetPositionField(standing);
            EditorGUILayout.Space();
            DrawAnimationSpeedField(standing);
            EditorGUILayout.Space();
            DrawBlockingAnimationField(standing);
        }

        private void DrawCharacterNameField(StoryCharacterStanding standing)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Character Name:", EditorStyles.boldLabel, GUILayout.Width(120));
            standing.Name = EditorGUILayout.TextField(standing.Name ?? "", GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawExpressionField(StoryCharacterStanding standing)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Expression:", EditorStyles.boldLabel, GUILayout.Width(120));
            standing.Expression = EditorGUILayout.TextField(standing.Expression ?? "", GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAnimationField(StoryCharacterStanding standing)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Animation:", EditorStyles.boldLabel, GUILayout.Width(120));
            standing.Animation = (StoryCharacterStanding.AnimationType)EditorGUILayout.EnumPopup(standing.Animation, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTargetPositionField(StoryCharacterStanding standing)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Position:", EditorStyles.boldLabel, GUILayout.Width(120));
            standing.TargetPosition = (StoryCharacterStanding.TargetPositionType)EditorGUILayout.EnumPopup(standing.TargetPosition, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAnimationSpeedField(StoryCharacterStanding standing)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Animation Speed:", EditorStyles.boldLabel, GUILayout.Width(120));
            standing.AnimationSpeed = EditorGUILayout.FloatField(standing.AnimationSpeed, GUILayout.Width(150));
            if (standing.AnimationSpeed < 0) standing.AnimationSpeed = 0;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBlockingAnimationField(StoryCharacterStanding standing)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Is Blocking Animation:", EditorStyles.boldLabel, GUILayout.Width(120));
            standing.IsBlockingAnimation = EditorGUILayout.Toggle(standing.IsBlockingAnimation, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            if (standing.IsBlockingAnimation)
            {
                EditorGUILayout.HelpBox("This animation will block the story progression until it completes.", MessageType.Info);
            }
        }
    }
}