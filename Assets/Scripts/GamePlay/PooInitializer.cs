using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class PooInitializer
{
    public static IEnumerator AsyncLoadPool()
    {
        Debug.Log("Loading EffectAsset for pooling...");

        var loadHandle = Addressables.LoadAssetAsync<EffectAsset>(AssetPath.EffectAsset);
        yield return loadHandle;

        var effectAsset = loadHandle.Result;
        var effectMap = new Dictionary<EffectType, GameObject>();
        foreach (var effectEntry in effectAsset.EffectAssets)
        {
            effectMap[effectEntry.EffectType] = effectEntry.EffectPrefab;
        }

        PoolManager.Instance.RegisterPool<EffectType, IEffect>(effectMap, 10);
    }
}
