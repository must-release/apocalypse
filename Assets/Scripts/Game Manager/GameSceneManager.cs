using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    private GameObject sceneObjects;
    private AsyncOperation asyncLoad;
    private SceneAssets waitingAssets;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public bool IsSceneReady()
    {
        if (asyncLoad == null && waitingAssets == null)
            return true;
        else
            return false;
    }

    public SceneAssets GetLoadedAssets()
    {
        return waitingAssets;
    }

    // Start scene loading
    public void StartSceneLoading(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    // Load scene asynchronously
    IEnumerator LoadSceneAsync(string sceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // wait until load is complete
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        asyncLoad = null;

        // After scene is loaded, load scene assets
        LoadAssetsAsync();
    }

    // Load scene assets asynchronously
    IEnumerator LoadAssetsAsync()
    {
        // When loading title scene, no need to load assets
        if (PlayerManager.Instance.PlayerData == null)
        {
            yield break;
        }

        // Prepare containter for scene assets
        sceneObjects = new GameObject("Scene Objects");
        sceneObjects.transform.position = Vector3.zero;
        sceneObjects.SetActive(false);
        waitingAssets = new SceneAssets();

        // Load assets
        LoadMaps();
        LoadPlayer();

    }

    // Load two maps when loading the scene
    public void LoadMaps()
    {
        UserData data = PlayerManager.Instance.PlayerData;
        string map1 = "MAP_" + data.CurrentStage.ToString() + '_' + data.CurrentMap;
        string map2 = "MAP_" + data.CurrentStage.ToString() + '_' + (data.CurrentMap + 1);

        StartCoroutine(AsyncLoadMaps(map1, map2));
    }

    // Load a single map when moving to next map
    public void LoadMap(int stageNum)
    {

    }

    // Load two maps asynchronously
    IEnumerator AsyncLoadMaps(string map1, string map2)
    {
        // Load first map
        AsyncOperationHandle<GameObject> first = Addressables.InstantiateAsync(map1, sceneObjects.transform);
        yield return first;
        if (first.Status == AsyncOperationStatus.Succeeded)
        {
            waitingAssets.currentMap = first.Result;
        }

        // Load second map
        AsyncOperationHandle<GameObject> second = Addressables.InstantiateAsync(map2, sceneObjects.transform);
        yield return second;
        if (second.Status == AsyncOperationStatus.Succeeded)
        {
            waitingAssets.nextMap = second.Result;
        }

        // Check error
        if (waitingAssets.currentMap == null || waitingAssets.nextMap == null)
            Debug.Log("Map Load Error");
    }

    // Start to load player prefab
    public void LoadPlayer()
    {
        string key = "Characters/Player";
        Addressables.InstantiateAsync(key, sceneObjects.transform).Completed += AsyncLoadPlayer;
    }

    // When player is loaded, return player instance to GameSceneManager
    private void AsyncLoadPlayer(AsyncOperationHandle<GameObject> playerInstance)
    {
        if (playerInstance.Status == AsyncOperationStatus.Succeeded)
        {
            waitingAssets.playerCharacter = playerInstance.Result;
        }

        if (waitingAssets.playerCharacter == null)
            Debug.Log("Player Load Error");
    }

}

public class SceneAssets
{
    public GameObject playerCharacter;
    public GameObject currentMap;
    public GameObject nextMap;

    public bool IsAssetReady()
    {
        return playerCharacter && currentMap && nextMap;
    }
}

