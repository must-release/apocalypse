using UnityEngine;
using System.Collections.Generic;

namespace AD.Story
{
    [CreateAssetMenu(fileName = "BackgroundCGAsset", menuName = "GameData/Story/BackgroundCGAsset")]
    public class BackgroundCGAsset : ScriptableObject
    {
        public List<BackgroundCGEntry> BackgroundCGAEntries;

        private void OnValidate()
        {
            foreach (var BGEntry in BackgroundCGAEntries)
            {
                Debug.Assert(BGEntry.Chapter != ChapterType.ChapterTypeCount, $"Chapter of the background sprites must be set.");
            }
        }
    }


    [System.Serializable]
    public class BackgroundCGEntry
    {
        public ChapterType Chapter;
        public List<Sprite> BackgroundSprites;
    }    
}

