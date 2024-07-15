using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class GameSceneView : MonoBehaviour
{
    public static GameSceneView Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlaceGameObjects()
    {
        StartCoroutine(PlaceGameObjectsAsync());
    }

    IEnumerator PlaceGameObjectsAsync()
    {
        // Wait for scene assets to be loaded
        while (true)
        {
            SceneAssets assets = GameSceneManager.Instance.GetLoadedAssets();
            if (assets != null)
            {
                if (assets.IsAssetReady()) break;
            }

            yield return null;
        }


    }

}