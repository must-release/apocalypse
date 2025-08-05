using UnityEngine;
using UnityEditor;
using AD.Audio;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace StoryEditor.UI
{
    public class SFXEditor : IStoryEntryEditor
    {
        /****** Public Members ******/

        public void Draw(EditorStoryEntry entry)
        {
            var storySFX = entry.AsSFX();
            if (null == storySFX) return;

            DrawSFXEditor(storySFX);
        }

        
        /****** Private Members ******/

        private SFXAsset _sfxAsset;

        private void DrawSFXEditor(StorySFX storySFX)
        {
            EditorGUILayout.LabelField("SFX Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            LoadSFXAssetIfNeeded();
            DrawSFXNameSelection(storySFX);
        }

        private void LoadSFXAssetIfNeeded()
        {
            if (null == _sfxAsset)
            {
                _sfxAsset = Addressables.LoadAssetAsync<SFXAsset>(AD.Audio.AssetPath.SFX).WaitForCompletion();
            }
        }

        private void DrawSFXNameSelection(StorySFX storySFX)
        {
            if (null == _sfxAsset)
            {
                EditorGUILayout.HelpBox("SFXAsset not found. Please create one in the project.", MessageType.Error);
                return;
            }

            if (null == _sfxAsset.SFXList || 0 == _sfxAsset.SFXList.Count)
            {
                EditorGUILayout.HelpBox("No SFX clips found in SFXAsset.", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("SFX:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var availableSFXs = _sfxAsset.SFXList.Where(clip => null != clip).ToArray();
            var sfxNames = availableSFXs.Select(clip => clip.name).ToArray();
            
            var currentIndex = System.Array.IndexOf(sfxNames, storySFX.SFXName);
            if (currentIndex == -1 && 0 < sfxNames.Length)
            {
                currentIndex = 0;
                storySFX.SFXName = sfxNames[0];
            }

            var newIndex = EditorGUILayout.Popup(currentIndex, sfxNames, GUILayout.Width(200));
            if (newIndex != currentIndex && 0 <= newIndex && newIndex < sfxNames.Length)
            {
                storySFX.SFXName = sfxNames[newIndex];
            }
            
            EditorGUILayout.EndHorizontal();

            // Show selected SFX info
            if (0 <= currentIndex && currentIndex < availableSFXs.Length)
            {
                var selectedSFX = availableSFXs[currentIndex];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox($"Selected: {selectedSFX.name} ({selectedSFX.length:F1}s)", MessageType.Info);  
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}