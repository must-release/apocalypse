using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Framework : MonoBehaviour
{
    private DevConfig _devConfig = null;    

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Main()
    {
        Assert.IsTrue(null == GameObject.Find("Framework"), "Framework instance already exists.");

        GameObject framework = new GameObject("Framework");
        framework.AddComponent<Framework>();
        DontDestroyOnLoad(framework);
    }

    private IEnumerator Start()
    {
        Debug.Log("Framework Init Start");

        // Do not change the order of these calls
        yield return LoadDevConfig();
        yield return LoadSystems();

        Debug.Log("Framework Init Complete");

        SubmitStartEvent();
    }

    private IEnumerator LoadDevConfig()
    {
        Debug.Log("Loading DevConfig...");
        
        AsyncOperationHandle<DevConfig> loadHandle = Addressables.LoadAssetAsync<DevConfig>(AssetPath.DevConfig);
        yield return loadHandle;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            _devConfig = loadHandle.Result;
            Debug.Log("Load DevConfig Complete : [" + AssetPath.DevConfig + "]");
        }
        else
        {
            Debug.LogError("Load DevConfig Failed : [" + AssetPath.DevConfig + "]");
            yield break;
        }
    }

    private IEnumerator LoadSystems()
    {
        Debug.Log("Loading Systems...");

        AsyncOperationHandle<SystemAsset> loadHandle = Addressables.LoadAssetAsync<SystemAsset>(AssetPath.SystemAsset);
        yield return loadHandle;

        if (loadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load SystemAsset");
            yield break;
        }

        SystemAsset group = loadHandle.Result;

        foreach (var entry in group.SystemAssets)
        {
            GameObject systemObject = Instantiate(entry.SystemPrefab, transform);
            systemObject.name = entry.SystemPrefab.name;
            Debug.Log("Instantiated System Prefab: " + entry.SystemPrefab.name);

            if (systemObject.TryGetComponent(out IAsyncLoadObject asyncObject))
            {
                yield return new WaitUntil(() => asyncObject.IsLoaded);
            }
        }

        Debug.Log("All System Prefabs Loaded.");
    }


    private void SubmitStartEvent()
    {
        Assert.IsTrue(null != _devConfig, "DevConfig not loaded");

        if (null != _devConfig.StartEvent)
        {
            var startEvent = GameEventFactory.CreateFromInfo(_devConfig.StartEvent);
            GameEventManager.Instance.Submit(startEvent);
        }
        else
        {
            Debug.Log("There is no Starting Event.");
        }
    }
}