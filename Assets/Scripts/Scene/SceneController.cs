using UnityEngine;
using System.Collections;

/* 
 * SceneConroller initializes maps when starting the StageScene.
 * It also creates next map for the player. */


public class SceneController : MonoBehaviour
{
    /****** Public Members ******/

    public static SceneController Instance { get; private set; }

    public bool IsSceneLoading { get; private set; }


    public void LoadGameScene(SceneEnums.SceneName loadingScene) 
    { 
        StartCoroutine(AsyncLoadGameScene(loadingScene)); 
    }

    public void ActivateGameScene()
    {
        UtilityManager.Instance.ResetUtilityTools();
        SceneObjectLoader.Instance.SetActiveSceneObjects(true);
    }

    public Transform FindPlayerTransform()
    {
        return SceneObjectLoader.Instance.Player;
    }


    /****** Private Members ******/

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
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

        AsyncOperation asyncLoad = SceneObjectLoader.Instance.AsyncLoadScene(loadingScene);
        yield return new WaitUntil(() => asyncLoad.isDone);

        yield return SceneObjectLoader.Instance.LoadAssets();

        SceneObjectPlacer.Instance.PlaceSceneObjects();

        IsSceneLoading = false;
    }
}

