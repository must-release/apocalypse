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

	// Load StageScene
	public void LoadStage()
	{
		SceneManager.LoadScene(stageSceneName);

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
		while(EventManager.Instance.CurrentEvent != null)
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

		// Place assets at the map
		PlaceMaps();
		PlaceCharacter();

		// If there is a event to be played, start the event
		IEvent startingEvent = GameManager.Instance.PlayerData.startingEvent;
		if(startingEvent != null)
		{
			EventManager.Instance.PlayEvent(startingEvent);
		}

    }


	// Place Maps at the right Place
	public void PlaceMaps()
	{
        Instantiate(currentMap);
        Instantiate(nextMap);
    }

	public void PlaceCharacter()
	{

	}
}

