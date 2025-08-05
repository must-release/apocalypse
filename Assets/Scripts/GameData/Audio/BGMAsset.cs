using System.Collections.Generic;
using UnityEngine;

namespace AD.Audio
{
    [CreateAssetMenu(fileName = "BGMAsset", menuName = "GameData/Audio/BGMAsset")]
    public class BGMAsset : ScriptableObject
    {
        public List<AudioClip> BGMList;

        private void OnValidate()
        {
            BGMList.ForEach(clip => Debug.Assert(null != clip, "BGM clip cannot be null"));
        }
    }
}

