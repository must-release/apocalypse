using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class PoolInitializer
{
    /****** Public Members ******/

    public static IEnumerator AsyncLoadPool()
    {
        yield return AsyncLoadEffectAsset();
    }

    
    /****** Private Members ******/

    private static IEnumerator AsyncLoadEffectAsset()
    {
        Logger.Write(LogCategory.AssetLoad, "Loading EffectAsset for pooling.", LogLevel.Log, true);

        var loadHandle = Addressables.LoadAssetAsync<EffectAsset>(AssetPath.EffectAsset);
        yield return loadHandle;

        var effectAsset = loadHandle.Result;
        foreach (var effectEntry in effectAsset.EffectAssets)
        {
            yield return PoolManager.Instance.AsyncRegisterPool<EffectType, IEffect>(effectEntry.EffectType, effectEntry.EffectPrefab, 
                effectEntry.PoolCount, $"EffectPool/{effectEntry.EffectType}");
        }

        Logger.Write(LogCategory.AssetLoad, "EffectAsset load complete.", LogLevel.Log, true);
    }


}
