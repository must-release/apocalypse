using System;
using System.Collections;
using AssetEnums;
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

        // if splash screen is enabled, notify it
        var splashController = FindObjectOfType<SplashScreen>();
        splashController?.OnFrameworkInitialized();
    }

    private IEnumerator LoadDevConfig()
    {
        Debug.Log("Loading DevConfig...");

        string assetPath = DevConfig.AssetPath;
        AsyncOperationHandle<DevConfig> loadHandle = Addressables.LoadAssetAsync<DevConfig>(assetPath);
        yield return loadHandle;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            _devConfig = loadHandle.Result;
            Debug.Log("Load DevConfig Complete : [" + assetPath + "]");
        }
        else
        {
            Debug.LogError("Load DevConfig Failed : [" + assetPath + "]");
            yield break;
        }
    }

    private IEnumerator LoadSystems()
    {
        Debug.Log("Loading Systems...");

        foreach (SystemAsset.AssetName system in Enum.GetValues(typeof(SystemAsset.AssetName)))
        {
            string assetPath = SystemAsset.PathPrefix + system.ToString();
            AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(assetPath);
            yield return loadHandle;

            GameObject systemObject;
            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                systemObject = Instantiate(loadHandle.Result, transform);
                systemObject.name = loadHandle.Result.name;
                Debug.Log("Load Complete : [" + assetPath + "]");
            }
            else
            {
                Debug.LogError("Load Failed : [" + assetPath + "]");
                yield break;
            }

            if ( systemObject.transform.TryGetComponent(out IAsyncLoadObject asyncObject) )
            {
                yield return new WaitUntil(() => asyncObject.IsLoaded());
            }
        }

        Debug.Log("All System Load is Complete.");
    }
}