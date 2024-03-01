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
        }
    }

    // Check if current PlayerData is new game data
    public bool IsNewGameData()
    {
        IEvent curEvent = PlayerData.startingEvent;

        if (curEvent == null)
        {
            return false;
        }
        else if (curEvent.EventType == IEvent.TYPE.STORY)
        {
            StoryEvent story = (StoryEvent)curEvent;
            if (story.stage == UserData.STAGE.TUTORIAL && story.storyNum == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(FindObjectOfType<Canvas>().gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
