﻿using UnityEngine;
using System.Linq;

public class ScenePlacer : MonoBehaviour
{
    public static ScenePlacer Instance;

    private float cameraHeight;
    private float cameraWidth;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Get Camera size
        cameraHeight = 2f * Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;
    }

    // Move scene objects to the right place
    public void PlaceSceneObjects()
    {
        // Check current scene
        SceneType currentScene = SceneLoader.Instance.CurrentScene;

        switch (currentScene)
        {
            case SceneType.TitleScene:
                PlaceTitleSceneObjects();
                break;
            case SceneType.StageScene:
                PlaceStageSceneObjects();
                break;
            default:
                Debug.Log("Wrong Scene Type");
                break;
        }
    }

    // Move title scene objects to the right place
    private void PlaceTitleSceneObjects()
    {
        return;
    }


    // Move stage scene objects to the right place
    private void PlaceStageSceneObjects()
    {
        MapInfo firstMap = SceneLoader.Instance.Maps.ElementAt(0);
        // MapInfo secondMap = GameSceneModel.Instance.Maps.ElementAt(1);

        // Place first map
        PlaceMap(firstMap);

        // Place second map
        // PlaceMap(firstMap, secondMap);

        // Place player at the first map
        PlacePlayer(firstMap);
    }

    // Place first map according to camera
    private void PlaceMap(MapInfo placingMap)
    {
        placingMap.map.position = Vector3.zero;
            // new Vector3((placingMap.size.x - cameraWidth) / 2, (placingMap.size.y - cameraHeight) / 2);
    }

    // Place second map based on first map
    public void PlaceMap(MapInfo firstmap, MapInfo secondMap)
    {
        secondMap.map.position =
            new Vector3(firstmap.size.x - (cameraWidth - secondMap.size.x) / 2, (secondMap.size.y - cameraHeight) / 2);
    }

    // Place player at the starting point of the current map
    private void PlacePlayer(MapInfo spawnMap)
    {
        SceneLoader.Instance.Player.position = spawnMap.startingPoint.position;
    }
}