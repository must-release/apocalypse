using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using SceneEnums;

/* 
 * SceneConroller initializes maps when starting the StageScene.
 * It also creates next map for the player. */

public class GameSceneController : MonoBehaviour
{
	public static GameSceneController Instance { get; private set; }

	public bool IsSceneLoading { get; set; }

	private string stageObjectsName = "Stage Objects";
	private string startingPointName = "Starting Point";
	private GameObject stageObjects;
	private GameObject player;

	void Awake()
	{
		if (Instance == null)
		{
            Instance = this;
        }
	}

    // Load game scene
    public void LoadGameScene(SCENE loadingScene)
	{
		StartCoroutine(AsyncLoadGameScene(loadingScene));
	}
	IEnumerator AsyncLoadGameScene(SCENE loadingScene)
	{
        // Start scene loading
        IsSceneLoading = true;

		// Load Scene
        AsyncOperation asyncLoad = GameSceneModel.Instance.AsyncLoadScene(loadingScene);
        while (!asyncLoad.isDone) { yield return null; }

		// Load Assets
	    yield return GameSceneModel.Instance.LoadAssets();

        // Place assets
        //PlaceMaps();
        //PlaceCharacter();

        // Complete scene loading
        IsSceneLoading = false;
    }

	public void ActivateGameScene()
	{

	}

	//// Place Maps at the right Place
	//public void PlaceMaps()
	//{
	//	Vector2 curMapSize = currentMap.GetComponent<BoxCollider2D>().size;
	//	Vector2 nextMapSize = nextMap.GetComponent<BoxCollider2D>().size;
	//	Vector3 playerPos = currentMap.transform.Find(startingPointName).transform.position;

 //       // Place maps based on there left corner.
 //       currentMap.transform.position =
	//		new Vector3((curMapSize.x - cameraWidth) / 2, (curMapSize.y - cameraHeight) / 2);
	//	nextMap.transform.position =
	//		new Vector3(curMapSize.x - (cameraWidth - nextMapSize.x) / 2, (nextMapSize.y - cameraHeight) / 2);
 //   }

	//// Place character at the starting point of the current map
	//public void PlaceCharacter()
	//{
	//	Vector3 playerPos = currentMap.transform.Find(startingPointName).transform.position;

	//	player.transform.position = playerPos;
	//}
}

