﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/* 
 * StageManager initializes maps when starting the StageScene.
 * It also creates next map for the player. */

public class GameSceneManager : MonoBehaviour
{
	public static GameSceneManager Instance { get; private set; }

	public bool IsStageReady { get; set; }


	private string stageSceneName = "StageScene";
	private string titleSceneName = "TitleScene";
	private string stageObjectsName = "Stage Objects";
	private string startingPointName = "Starting Point";
	private float cameraHeight;
	private float cameraWidth;
	private GameObject stageObjects;
	private GameObject currentMap;
	private GameObject nextMap;
	private GameObject player;

	void Awake()
	{
		if (Instance == null)
		{
            Instance = this;
        }
	}

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Load StageScene
    public void LoadStage()
	{
		IsStageReady = false;
		StoryPlayer.Instance.StopPlaying();
		SceneManager.LoadScene(stageSceneName);
	}

	// Exit StageScene
	public void GoTitle() { StartCoroutine(LoadTitleSceneAsync()); }
    IEnumerator LoadTitleSceneAsync()
    {
		IsStageReady = false;
        StoryPlayer.Instance.StopPlaying();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(titleSceneName);

        // Play loading
        UIManager.Instance.ChangeState(UIManager.STATE.LOADING, true);

        // wait until loading is complete
        while (!asyncLoad.isDone)
        {
			yield return null;
        }

        // Show Title UI
        UIManager.Instance.ChangeState(UIManager.STATE.TITLE, true);
    }


    // When StageScene is loaded, start event and initialize assets
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
        // Activated only in StageScene
        if (scene.name != stageSceneName) { return; }

        // Get Camera size
        cameraHeight = 2f * Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;

        // Find Stage Objects gameobject
        stageObjects  = GameObject.Find(stageObjectsName);

		// Play starting event in the player's data
		UserData data = GameManager.Instance.PlayerData;
		EventManager.Instance.PlayEvent(data.StartingEvent);

		// Start loading assets
		DataManager.Instance.LoadMaps();
		DataManager.Instance.LoadPlayer();
		
    }

	// When maps are loaded, set there parents as Stage Objects
	public void OnMapsLoadComplete(GameObject map1, GameObject map2)
	{
		currentMap = map1;
		currentMap.transform.SetParent(stageObjects.transform);
		nextMap = map2;
		nextMap.transform.SetParent(stageObjects.transform);

		OnAssetsLoadComplete();
    }

	// When player is loaded, set its parents as Stage Objects
	public void onPlayerLoadComplete(GameObject playerInstance)
	{
		player = playerInstance;
		player.transform.SetParent(stageObjects.transform);

		OnAssetsLoadComplete();
	}

	public void OnAssetsLoadComplete()
	{
		if (currentMap == null || nextMap == null || player == null)
			return;

		// Place assets
		PlaceMaps();
		PlaceCharacter();

        // Alarm that stage scene is ready
        IsStageReady = true;
	}

	// Place Maps at the right Place
	public void PlaceMaps()
	{
		Vector2 curMapSize = currentMap.GetComponent<BoxCollider2D>().size;
		Vector2 nextMapSize = nextMap.GetComponent<BoxCollider2D>().size;
		Vector3 playerPos = currentMap.transform.Find(startingPointName).transform.position;

        // Place maps based on there left corner.
        currentMap.transform.position =
			new Vector3((curMapSize.x - cameraWidth) / 2, (curMapSize.y - cameraHeight) / 2);
		nextMap.transform.position =
			new Vector3(curMapSize.x - (cameraWidth - nextMapSize.x) / 2, (nextMapSize.y - cameraHeight) / 2);
    }

	// Place character at the starting point of the current map
	public void PlaceCharacter()
	{
		Vector3 playerPos = currentMap.transform.Find(startingPointName).transform.position;

		player.transform.position = playerPos;
	}

	// Active/Inactive Stage Objects gameobject
	public void SetStageObjectsActive(bool control)
	{
		if(stageObjects)
			stageObjects.SetActive(control);
	}

}
