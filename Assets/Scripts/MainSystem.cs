using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using AssetEnums;
using TMPro;

public class MainSystem : MonoBehaviour
{
    private static MainSystem _instance = null;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(InitializeSystems());
    }

    private IEnumerator InitializeSystems()
    {
        Debug.Log("Start System Initialization...");

        foreach (SystemAsset.AssetName system in Enum.GetValues(typeof(SystemAsset.AssetName)))
        {
            string assetPath = SystemAsset.PathPrefix + system.ToString();
            AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(assetPath);
            yield return loadHandle;

            GameObject systemObject;
            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                systemObject = Instantiate(loadHandle.Result, transform);
                Debug.Log("Initialization Complete : [" + assetPath + "]");
            }
            else
            {
                Debug.LogError("Initialization Failed : [" + assetPath + "]");
                yield break;
            }

            if ( systemObject.transform.TryGetComponent(out IAsyncLoadObject asyncObject) )
            {
                yield return new WaitUntil(() => asyncObject.IsLoaded());
            }
        }

        Debug.Log("All System Initialization Complete. Move to Main Scene");

        var splashScreenEventInfo = Addressables.LoadAssetAsync<GameEventInfo>(SequentialEventInfoAsset.Common.SplashScreen);
        yield return splashScreenEventInfo;
        if (splashScreenEventInfo.Status == AsyncOperationStatus.Succeeded)
        {
            var splashScreenEvent = GameEventFactory.CreateFromInfo(splashScreenEventInfo.Result);
            GameEventManager.Instance.Submit(splashScreenEvent);
        }
        else
        {
            Debug.LogError("Failed to load Splash Screen Event Info");
        }
    }
}