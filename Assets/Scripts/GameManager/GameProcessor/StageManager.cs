using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/* 
 * StageManager initializes maps when starting the StageScene.
 * It also creates next map for the player. */

public class StageManager : MonoBehaviour
{
	public static StageManager Instance { get; private set; }


	private string stageSceneName = "StageScene";
	private GameObject currentMap;
	private GameObject nextMap;
	private GameObject player;

	void Awake()
	{
		if (Instance == null)
			Instance = this;

		SceneManager.sceneLoaded += OnStageLoaded;
	}

	// Load assets for the StageScene to be started
	public void LoadStage()
	{
		DataManager.Instance.LoadMaps(); // Load two maps asynchronously
    }

	// Assets are now loaded
	public void OnLoadComplete(GameObject curMap, GameObject nxtMap)
	{
		// initialize assets
		currentMap = curMap;
		nextMap = nxtMap;

		// Check if it is new game
		if (GameManager.Instance.IsNewGameData())
		{
			StartCoroutine(AsyncStageLoad());
		}
		else
		{
			SceneManager.LoadScene(stageSceneName);
		}

    }

	// Wait for start story to end
	IEnumerator AsyncStageLoad()
	{
		while(GameManager.Instance.PlayerData.startingEvent != null)
		{
			yield return null;
		}

		SceneManager.LoadScene(stageSceneName);
	}

	// When StageScene is loaded, initialize assets
	public void OnStageLoaded(Scene scene, LoadSceneMode mode)
	{
		// Activates only in StageScene
		if(scene.name != stageSceneName) { return; }

		PlaceMaps();
    }


	// Place Maps at the right Place
	public void PlaceMaps()
	{
        Instantiate(currentMap);
        Instantiate(nextMap);
    }
}

