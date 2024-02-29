using UnityEngine;
using System.Collections;

/* 
 * StageManager initializes maps when starting the StageScene.
 * It also creates next map for the player. */

public class StageManager : MonoBehaviour
{
	public static StageManager Instance { get; private set; }

	private GameObject currentMap;
	private GameObject nextMap;

	void Awake()
	{
		if (Instance == null)
			Instance = this;
	}

	// Load assets for the StageScene to be started
	public void LoadStage()
	{
		DataManager.Instance.LoadMaps(); // Load two maps asynchronously
    }

	public void OnLoadComplete(GameObject curMap, GameObject nxtMap)
	{
		currentMap = curMap;
		nextMap = nxtMap;

		Instantiate(currentMap, new Vector3(0, 0, 0), Quaternion.identity);
        Instantiate(nextMap, new Vector3(0, 10, 0), Quaternion.identity);

    }
}

