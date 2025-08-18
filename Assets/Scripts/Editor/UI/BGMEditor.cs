using UnityEngine;
using UnityEditor;
using AD.Audio;
using AD.Story;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace StoryEditor.UI
{
    public class BGMEditor : IStoryEntryEditor
    {
        /****** Public Members ******/

        public void Draw(EditorStoryEntry entry)
        {
            var bgm = entry.AsBGM();
            if (null != bgm)
            {
                DrawBGMEditor(bgm);
            }
        }

        
        /****** Private Members ******/

        private BGMAsset _bgmAsset;

        private void DrawBGMEditor(StoryBGM bgm)
        {
            EditorGUILayout.LabelField("BGM Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawActionSelection(bgm);
            EditorGUILayout.Space();

            if (bgm.Action == StoryBGM.BGMAction.Start)
            {
                LoadBGMAssetIfNeeded();
                DrawBGMNameSelection(bgm);
                EditorGUILayout.Space();
                DrawIsLoopField(bgm);
                EditorGUILayout.Space();
            }

            DrawFadeDurationField(bgm);
        }

        private void LoadBGMAssetIfNeeded()
        {
            if (null == _bgmAsset)
            {
                _bgmAsset = Addressables.LoadAssetAsync<BGMAsset>(AD.Audio.AssetPath.BGM).WaitForCompletion();
            }
        }

        private void DrawActionSelection(StoryBGM bgm)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Action:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newAction = (StoryBGM.BGMAction)EditorGUILayout.EnumPopup(bgm.Action, GUILayout.Width(100));
            if (newAction != bgm.Action)
            {
                bgm.Action = newAction;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBGMNameSelection(StoryBGM bgm)
        {
            if (null == _bgmAsset)
            {
                EditorGUILayout.HelpBox("BGMAsset not found. Please create one in the project.", MessageType.Error);
                return;
            }

            if (null == _bgmAsset.BGMList || 0 == _bgmAsset.BGMList.Count)
            {
                EditorGUILayout.HelpBox("No BGM clips found in BGMAsset.", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("BGM:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var availableBGMs = _bgmAsset.BGMList.Where(clip => null != clip).ToArray();
            var bgmNames = availableBGMs.Select(clip => clip.name).ToArray();
            
            var currentIndex = System.Array.IndexOf(bgmNames, bgm.BGMName);
            if (currentIndex == -1 && 0 < bgmNames.Length)
            {
                currentIndex = 0;
                bgm.BGMName = bgmNames[0];
            }

            var newIndex = EditorGUILayout.Popup(currentIndex, bgmNames, GUILayout.Width(200));
            if (newIndex != currentIndex && 0 <= newIndex && newIndex < bgmNames.Length)
            {
                bgm.BGMName = bgmNames[newIndex];
            }
            
            EditorGUILayout.EndHorizontal();

            // Show selected BGM info
            if (0 <= currentIndex && currentIndex < availableBGMs.Length)
            {
                var selectedBGM = availableBGMs[currentIndex];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox($"Selected: {selectedBGM.name} ({selectedBGM.length:F1}s)", MessageType.Info);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawFadeDurationField(StoryBGM bgm)
        {
            EditorGUILayout.BeginHorizontal();
            var labelText = bgm.Action == StoryBGM.BGMAction.Start ? "Fade In Duration:" : "Fade Out Duration:";
            EditorGUILayout.LabelField(labelText, EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newFadeDuration = EditorGUILayout.FloatField(bgm.FadeDuration, GUILayout.Width(100));
            if (newFadeDuration != bgm.FadeDuration)
            {
                bgm.FadeDuration = Mathf.Max(0f, newFadeDuration);
            }
            
            EditorGUILayout.LabelField("seconds", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawIsLoopField(StoryBGM bgm)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Is Loop:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newIsLoop = EditorGUILayout.Toggle(bgm.IsLoop, GUILayout.Width(50));
            if (newIsLoop != bgm.IsLoop)
            {
                bgm.IsLoop = newIsLoop;
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}