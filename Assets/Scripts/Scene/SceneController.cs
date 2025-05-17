using UnityEngine;
using System.Collections;
using NUnit.Framework;

/* 
 * SceneConroller initializes maps when starting the StageScene.
 * It also creates next map for the player. */


public class SceneController : MonoBehaviour
{
    /****** Public Members ******/

    public static SceneController Instance { get; private set; }

    public bool IsSceneLoading { get; private set; }
    public bool CanMoveToNextScene
    {
        get
        {
            Assert.IsTrue(null != _currentScene, "Current scene is not initialized.");

            return _currentScene.CanMoveToNextScene;
        }
    }


    public void LoadGameScene(SceneEnums.SceneName loadingScene) 
    { 
        StartCoroutine(AsyncLoadGameScene(loadingScene)); 
    }

    public void ActivateGameScene()
    {
        UtilityManager.Instance.ResetUtilityTools();
        SceneLoader.Instance.SetActiveSceneObjects(true);
    }

    public Transform FindPlayerTransform()
    {
        return SceneLoader.Instance.Player;
    }


    /****** Private Members ******/

    private IScene _currentScene = null;

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;

            FindCurrentScene();
        }
        else
        {
            Debug.LogWarning("SceneController already exists. Destroying the new instance.");
            Destroy(gameObject);
        }
    }

    private IEnumerator AsyncLoadGameScene(SceneEnums.SceneName loadingScene)
    {
        IsSceneLoading = true;

        AsyncOperation asyncLoad = SceneLoader.Instance.AsyncLoadScene(loadingScene);
        yield return new WaitUntil(() => asyncLoad.isDone);

        FindCurrentScene();

        yield return SceneLoader.Instance.LoadAssets();

        ScenePlacer.Instance.PlaceSceneObjects();

        IsSceneLoading = false;
    }

    private void FindCurrentScene()
    {
        MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var behaviour in behaviours)
        {
            if (behaviour is IScene scene)
            {
                _currentScene = scene;
                break;
            }
        }

        if (null == _currentScene)
        {
            Debug.LogWarning("No IScene implementation found in the current scene.");
        }
    }
}

