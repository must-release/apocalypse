using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    // Current data of the player
    public UserData PlayerData { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Prevent multiple audio listener
    private void Start()
    {
        GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = true;

    }

    // Get current stage & map info which player is in
    public (string, int) GetCurrentStageMapInfo()
    {
        return (PlayerData.CurrentStage.ToString(), PlayerData.CurrentMap);
    }
}
