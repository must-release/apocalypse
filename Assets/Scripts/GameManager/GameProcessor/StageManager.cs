using UnityEngine;
using System.Collections;

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

	public void LoadStage()
	{
		UserData data = GameManager.Instance.PlayerData;
		DataManager.Instance.LoadMap(data.currentStage, data.currentMap, data.currentMap + 1);
    }

	public void OnLoadComplete(GameObject curMap, GameObject nxtMap)
	{
		currentMap = curMap;
		nextMap = nxtMap;

		Instantiate(currentMap, new Vector3(0, 0, 0), Quaternion.identity);
        Instantiate(nextMap, new Vector3(0, 10, 0), Quaternion.identity);

    }
}

