using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class BackgroundCGModel
    {
        /****** Public Members ******/

        public BackgroundCGAsset BackgroundCGAsset { get; private set; }

        public async UniTask AsyncLoadBackgroundCGAsset()
        {
            if (null == BackgroundCGAsset)
            {
                var handle = Addressables.LoadAssetAsync<BackgroundCGAsset>(AssetPath.BackgroundCGAsset);
                await handle.ToUniTask();

                Debug.Assert(handle.Status == AsyncOperationStatus.Succeeded, "Failed to load BackgroundCGAsset.");

                BackgroundCGAsset = handle.Result;

                foreach (var entry in BackgroundCGAsset.BackgroundCGAEntries)
                {
                    _backgroundSprites[entry.Chapter] = new Dictionary<string, Sprite>();
                    foreach (var bgSprite in entry.BackgroundSprites)
                    {
                        _backgroundSprites[entry.Chapter][bgSprite.name] = bgSprite;
                    }
                }
            }
        }

        public Sprite GetBackgroundSprite(ChapterType chapter, string bgName)
        {
            Debug.Assert(null != BackgroundCGAsset, "BackgroundCGAsset is not loaded.");

            if (_backgroundSprites.TryGetValue(chapter, out var chapterSprites))
            {
                if (chapterSprites.TryGetValue(bgName, out Sprite sprite))
                {
                    return sprite;
                }
            }

            Debug.LogWarning($"Background sprite for '{bgName}' in chapter '{chapter}' not found.");
            return null;
        }


        /****** Private Members ******/

        private Dictionary<ChapterType, Dictionary<string, Sprite>> _backgroundSprites = new();
    }
}