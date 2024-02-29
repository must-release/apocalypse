using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Current data of the player
    private UserData _playerData;
    public UserData PlayerData
    {
        get { return _playerData; }

        // When data is modified, do AutoSave
        set
        {
            _playerData = value;
            DataManager.Instance.AutoSave(_playerData);
        }
    }

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

}
