using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Current data of the player
    public UserData PlayerData { get; set; }

    // Check if current PlayerData is new game data
    public bool IsNewGameData()
    {
        EventBase curEvent = GameEventRouter.Instance.CurrentEvent;

        if (curEvent == null)
        {
            return false;
        }
        else if (curEvent.EventType == EventBase.TYPE.STORY)
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Prevent multiple audio listener or EventSystem
    private void Start()
    {
        GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = true;
    }

}