using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    public SceneType CurrentScene { get; private set; }
    public GameObject SceneObjects { get; private set; }
    public Transform Player { get; private set; }
    public Queue<MapInfo> Maps { get; private set; }

    private string sceneObjectsName = "Scene Objects"; // In game scene, Scene Objects must exist
    private int mapCount = 4; // Capacity of map queue


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneObjects = transform.Find(sceneObjectsName).gameObject;
            Maps = new Queue<MapInfo>();
        }
    }

    // Activate or inactivate every scene objects
    public void SetActiveSceneObjects(bool value)
    {
        foreach (Transform child in SceneObjects.transform)
        {
            child.gameObject.SetActive(value);
        }
    }

    // Asynchronously load scene
    public AsyncOperation AsyncLoadScene(SceneType loadingScene)
    {
        CurrentScene = loadingScene;

        return SceneManager.LoadSceneAsync(GetSceneName(loadingScene));
    }

    // Load Assets
    public IEnumerator LoadAssets() 
    {
        // Destroy all previous scene objects
        foreach (Transform child in SceneObjects.transform)
        {
            Destroy(child.gameObject);
        }
        Maps.Clear();

        yield return LoadStageAssets();
    }

    // Load stage scene assets
    IEnumerator LoadStageAssets()
    {
        // Get map data
//         PlayerManager.Instance.GetStageMapInfo(out string stage, out int map);
//         string map1 = "Stage/" + stage + '_' + map;
//         string map2 = "Stage/" + stage + '_' + (map + 1);

        // Load first map
/*        yield return AsyncLoadStage(map1);*/

        // Load second map
        //yield return StartCoroutine(AsyncLoadStage(map2));

        // Load player objects
        yield return LoadPlayer();
    }

    // Load title scene assets
    IEnumerator LoadTitleAssets()
    {
        yield break;
    }

    // Load map and execute onComplete delegate
    IEnumerator LoadMap(string map)
    {

        AsyncOperationHandle<GameObject> loadingMap = Addressables.InstantiateAsync(map, SceneObjects.transform);
        yield return loadingMap;
        if (loadingMap.Status == AsyncOperationStatus.Succeeded)
        {
            // Load map in active state
            Transform loadedMap = loadingMap.Result.transform;

            // Wait for async loading of scene objects in the map
            foreach (var sceneObject in loadedMap.GetComponentsInChildren<IAsyncLoadObject>())
            {
                yield return new WaitUntil(()=>sceneObject.IsLoaded);
            }

            // Check if the Maps queue is at its maximum capacity
            if (Maps.Count >= mapCount)
            {
                Destroy(Maps.Dequeue().map); // Remove the oldest map
            }
            Maps.Enqueue(new MapInfo(loadedMap.gameObject));

            // Inactivate loaded map
            loadedMap.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Failed to load the map: " + map);
        }
    }

    // Load player prefab
    IEnumerator LoadPlayer()
    {
        AsyncOperationHandle<GameObject> player = Addressables.InstantiateAsync("Character/Player");
        yield return player;
        if (player.Status == AsyncOperationStatus.Succeeded)
        {
            Player = player.Result.transform;
            if (Player.TryGetComponent(out IAsyncLoadObject sceneObject))
            {
                yield return new WaitUntil(()=>sceneObject.IsLoaded);
            }
            Player.gameObject.SetActive(false);
            Player.parent = SceneObjects.transform;
        }
        else
        {
            Debug.LogError("Failed to load player");
        }

    }

    // Return scene name string
    private string GetSceneName(SceneType loadingScene)
    {
        switch (loadingScene)
        {
            case SceneType.TitleScene:
                return "TitleScene";
            case SceneType.StageScene:
                return "StageScene";
            default:
                return null;
        }
    }
}

// Contains map info
public class MapInfo
{
    public Transform map;
    public Vector2 size;
    public Transform startingPoint;

    public MapInfo(GameObject map)
    {
        this.map = map.transform;
        //size = map.GetComponent<BoxCollider2D>().size;
        startingPoint = map.transform.Find("Starting Point");
    }
}