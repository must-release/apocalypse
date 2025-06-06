using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "EffectAsset", menuName = "GameData/EffectAsset")]
public class EffectAsset : ScriptableObject
{
    public List<EffectEntry> EffectAssets;

    private void OnValidate()
    {
        
    }
}

[System.Serializable]
public class EffectEntry
{
    public EffectType EffectType;
    public AssetReferenceGameObject EffectPrefab;
    public int PoolCount;
}
