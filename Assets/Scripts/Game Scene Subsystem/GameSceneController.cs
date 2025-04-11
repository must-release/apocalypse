using UnityEngine;
using System.Collections;
using SceneEnums;
using UnityEngine.SceneManagement;

/* 
 * SceneConroller initializes maps when starting the StageScene.
 * It also creates next map for the player. */

public class GameSceneController : MonoBehaviour
{
	public static GameSceneController Instance { get; private set; }

	public bool IsSceneLoading { get; set; }

	void Awake()
	{
		if (Instance == null)
		{
            Instance = this;
        }
	}

    // Load game scene
    public void LoadGameScene(SceneEnums.Scene loadingScene) { StartCoroutine(AsyncLoadGameScene(loadingScene)); }
	IEnumerator AsyncLoadGameScene(SceneEnums.Scene loadingScene)
	{
        // Start scene loading
        IsSceneLoading = true;

		// Load Scene
        AsyncOperation asyncLoad = GameSceneModel.Instance.AsyncLoadScene(loadingScene);
        while (!asyncLoad.isDone) { yield return null; }

		// Load Assets
	    yield return GameSceneModel.Instance.LoadAssets();

		// Place assets
		GameSceneView.Instance.PlaceSceneObjects();

        // Complete scene loading
        IsSceneLoading = false;
    }

	// Activate loaded game scene
	public void ActivateGameScene()
	{
		// Reset utility tools
		UtilityManager.Instance.ResetUtilityTools();

		// Activate loaded scene objects
		GameSceneModel.Instance.SetActiveSceneObjects(true);
	}

	// Get Player Transform
	public Transform FindPlayerTransform()
	{
		return GameSceneModel.Instance.Player;
	}
}
