using UnityEngine;
using UnityEditor;
using AD.Story;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace StoryEditor.UI
{
    public class BackgroundCGEditor : IStoryEntryEditor
    {
        /****** Public Members ******/

        public void Draw(EditorStoryEntry entry)
        {
            var backgroundCG = entry.AsBackgroundCG();
            if (null == backgroundCG) return;

            LoadBackgroundCGAssetIfNeeded();
            
            if (null == _backgroundCGAsset)
            {
                EditorGUILayout.HelpBox("BackgroundCGAsset not found. Please create one in the project.", MessageType.Error);
                return;
            }

            DrawChapterSelection(backgroundCG);
            EditorGUILayout.Space();
            DrawImageNameSelection(backgroundCG);
            EditorGUILayout.Space();
            DrawImagePreview(backgroundCG);
        }

        
        /****** Private Members ******/

        private BackgroundCGAsset _backgroundCGAsset;

        private void LoadBackgroundCGAssetIfNeeded()
        {
            if (null == _backgroundCGAsset)
            {
                _backgroundCGAsset = Addressables.LoadAssetAsync<BackgroundCGAsset>(AD.Story.AssetPath.BackgroundCG).WaitForCompletion();
            }
        }

        private void DrawChapterSelection(StoryBackgroundCG backgroundCG)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Chapter:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var availableChapters = _backgroundCGAsset.BackgroundCGAEntries.Select(entry => entry.Chapter).ToArray();
            var chapterNames = availableChapters.Select(chapter => chapter.ToString()).ToArray();
            
            var currentIndex = System.Array.IndexOf(availableChapters, backgroundCG.Chapter);
            if (currentIndex == -1 && 0 < availableChapters.Length) 
            {
                currentIndex = 0;
                backgroundCG.Chapter = availableChapters[0];
            }

            var newIndex = EditorGUILayout.Popup(currentIndex, chapterNames, GUILayout.Width(150));
            if (newIndex != currentIndex && 0 <= newIndex && newIndex < availableChapters.Length)
            {
                backgroundCG.Chapter = availableChapters[newIndex];
                backgroundCG.ImageName = ""; // Reset image name when chapter changes
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawImageNameSelection(StoryBackgroundCG backgroundCG)
        {
            var chapterEntry = _backgroundCGAsset.BackgroundCGAEntries.FirstOrDefault(entry => entry.Chapter == backgroundCG.Chapter);
            if (null == chapterEntry || null == chapterEntry.BackgroundSprites || 0 == chapterEntry.BackgroundSprites.Count)
            {
                EditorGUILayout.HelpBox("No background sprites found for selected chapter.", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Image:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var availableSprites = chapterEntry.BackgroundSprites.Where(sprite => null != sprite).ToArray();
            var spriteNames = availableSprites.Select(sprite => sprite.name).ToArray();
            
            var currentIndex = System.Array.IndexOf(spriteNames, backgroundCG.ImageName);
            if (currentIndex == -1 && 0 < spriteNames.Length)
            {
                currentIndex = 0;
                backgroundCG.ImageName = spriteNames[0];
            }

            var newIndex = EditorGUILayout.Popup(currentIndex, spriteNames, GUILayout.Width(200));
            if (newIndex != currentIndex && 0 <= newIndex && newIndex < spriteNames.Length)
            {
                backgroundCG.ImageName = spriteNames[newIndex];
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawImagePreview(StoryBackgroundCG backgroundCG)
        {
            var chapterEntry = _backgroundCGAsset.BackgroundCGAEntries.FirstOrDefault(entry => entry.Chapter == backgroundCG.Chapter);
            if (null == chapterEntry || null == chapterEntry.BackgroundSprites)
            {
                return;
            }

            var selectedSprite = chapterEntry.BackgroundSprites.FirstOrDefault(sprite => null != sprite && sprite.name == backgroundCG.ImageName);
            if (null == selectedSprite)
            {
                if (false == string.IsNullOrEmpty(backgroundCG.ImageName))
                {
                    EditorGUILayout.HelpBox($"Image '{backgroundCG.ImageName}' not found in chapter {backgroundCG.Chapter}.", MessageType.Warning);
                }
                return;
            }

            EditorGUILayout.HelpBox($"Selected sprite: {selectedSprite.name}", MessageType.Info);
            
            if (null != selectedSprite.texture)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);
                var rect = GUILayoutUtility.GetRect(200, 150, GUILayout.ExpandWidth(false));
                EditorGUI.DrawPreviewTexture(rect, selectedSprite.texture);
            }
        }
    }
}