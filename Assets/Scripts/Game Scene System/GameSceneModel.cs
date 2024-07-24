using SceneEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class GameSceneModel : MonoBehaviour
{
    public static GameSceneModel Instance;

    public SCENE CurrentScene { get; private set; }
    public GameObject StageObjects { get; private set; }
    public GameObject Player { get; private set; }
    
    private string stageObjectsName = "Stage Objects";
    private GameObject currentMap;
    private GameObject nextMap;
    private float cameraHeight;
    private float cameraWidth;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Get Camera size
        cameraHeight = 2f * Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;
    }

    // Asyncronously load scene
    public AsyncOperation AsyncLoadScene(SCENE loadingScene)
    {
        CurrentScene = loadingScene;

        return SceneManager.LoadSceneAsync(GetSceneName(loadingScene));
    }

    // Load Assets
    public Coroutine LoadAssets() 
    {
        switch (CurrentScene)
        {
            case SCENE.TITLE:
                return StartCoroutine(LoadTitleAssets());
            case SCENE.STAGE:
                return StartCoroutine(LoadStageAssets());
            default:
                Debug.Log("Asset Load Fail : Invalid Scene");
                return null;
        }
    }

    // Load stage scene assets
    IEnumerator LoadStageAssets()
    {
        // Find container of the stage objects 
        StageObjects = GameObject.Find(stageObjectsName);


        /****** Load Maps ******/
        {
            UserData data = PlayerManager.Instance.PlayerData;
            string map1 = "MAP_" + data.CurrentStage.ToString() + '_' + data.CurrentMap;
            string map2 = "MAP_" + data.CurrentStage.ToString() + '_' + (data.CurrentMap + 1);

            // Load first map
            yield return StartCoroutine(LoadMap(map1, result => currentMap = result));

            // Load second map
            yield return StartCoroutine(LoadMap(map2, result => nextMap = result));
        }


        /***** Load Player Prefab *****/
        yield return LoadPlayer();
    }

    // Load title scene assets
    IEnumerator LoadTitleAssets()
    {
        yield break;
    }

    IEnumerator LoadMap(string map, System.Action<GameObject> onComplete)
    {

        AsyncOperationHandle<GameObject> loadingMap = Addressables.InstantiateAsync(map);
        yield return loadingMap;
        if (loadingMap.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject loadedMap = loadingMap.Result;
            loadedMap.SetActive(false);
            loadedMap.transform.parent = StageObjects.transform;
            onComplete?.Invoke(loadedMap);
        }
        else
        {
            Debug.LogError("Failed to load the map: " + map);
            onComplete?.Invoke(null);
        }
    }

    IEnumerator LoadPlayer()
    {
        AsyncOperationHandle<GameObject> player = Addressables.InstantiateAsync("Characters/Player");
        yield return player;
        if (player.Status == AsyncOperationStatus.Succeeded)
        {
            Player = player.Result;
            Player.SetActive(false);
            Player.transform.parent = StageObjects.transform;
        }
        else
        {
            Debug.LogError("Failed to load player");
        }

    }

    // Return scene name string
    private string GetSceneName(SCENE loadingScene)
    {
        switch (loadingScene)
        {
            case SCENE.TITLE:
                return "TitleScene";
            case SCENE.STAGE:
                return "StageScene";
            default:
                return null;
        }
    }
}

