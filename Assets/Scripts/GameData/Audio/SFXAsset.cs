using System.Collections.Generic;
using UnityEngine;

namespace AD.Audio
{
    [CreateAssetMenu(fileName = "SFXAsset", menuName = "GameData/Audio/SFXAsset")]
    public class SFXAsset : ScriptableObject
    {
        public List<AudioClip> SFXList;

        private void OnValidate()
        {
            SFXList.ForEach(clip => Debug.Assert(null != clip, "SFX clip cannot be null"));
        }
    }
}