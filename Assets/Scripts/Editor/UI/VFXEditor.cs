using UnityEngine;
using UnityEditor;

namespace StoryEditor.UI
{
    public class VFXEditor : IStoryEntryEditor
    {
        /****** Public Members ******/

        public void Draw(EditorStoryEntry entry)
        {
            var vfx = entry.AsVFX();
            if (null == vfx) return;

            DrawActionField(vfx);
            EditorGUILayout.Space();
            DrawDurationField(vfx);
        }
        
        
        /****** Private Members ******/

        private void DrawActionField(StoryVFX vfx)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("VFX Type:", EditorStyles.boldLabel, GUILayout.Width(120));

            var newVFXType = (StoryVFX.VFXType)EditorGUILayout.EnumPopup(vfx.VFX, GUILayout.Width(150));
            if (newVFXType != vfx.VFX)
            {
                vfx.VFX = newVFXType;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawDurationField(StoryVFX vfx)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Duration:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newDuration = EditorGUILayout.FloatField(vfx.Duration, GUILayout.Width(100));
            if (newDuration != vfx.Duration)
            {
                vfx.Duration = Mathf.Max(0f, newDuration);
            }
            
            EditorGUILayout.LabelField("seconds", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }
    }
}