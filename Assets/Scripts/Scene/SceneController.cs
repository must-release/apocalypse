using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 
 * SceneConroller initializes maps when starting the StageScene.
 * It also creates next map for the player. 
 */


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
    public Transform PlayerTransform
    {
        get
        {
            Assert.IsTrue(null != _currentScene, "Current scene is not initialized.");
            return _currentScene.PlayerTransform;
        }
    }

    public void LoadGameScene(SceneType loadingScene) 
    {
        AsyncLoadGameScene(loadingScene).Forget();
    }

    public void ActivateGameScene()
    {
        Assert.IsTrue(null != _currentScene, "Current scene is not initialized.");

        UtilityManager.Instance.ResetUtilityTools();
        _currentScene.ActivateScene();
    }


    /****** Private Members ******/

    private IScene _currentScene;

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;

            _currentScene = FindCurrentScene();
        }
        else
        {
            Debug.LogWarning("SceneController already exists. Destroying the new instance.");
            Destroy(gameObject);
        }
    }

    private async UniTask AsyncLoadGameScene(SceneType loadingScene)
    {
        IsSceneLoading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadingScene.ToString());
        await asyncLoad.ToUniTask();

        _currentScene = FindCurrentScene();
        await _currentScene.AsyncInitializeScene();

        IsSceneLoading = false;
    }

    private IScene FindCurrentScene()
    {
        MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var behaviour in behaviours)
        {
            if (behaviour is IScene scene)
            {
                return scene;
            }
        }

        Logger.Write(LogCategory.GameScene, "No IScene implementation found in the current scene.", LogLevel.Error, true);

        return null;
    }
}

