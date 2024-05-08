using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public enum STATE { TITLE, CONTROL, STORY, EMPTY, LOADING, SAVE, LOAD, PAUSE }

    private STATE currentUI; // UI using right now
    private Stack<STATE> savedUIs; // UIs which will be used again

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // initalize current UI to title UI
        currentUI = STATE.TITLE;

        savedUIs = new Stack<STATE>();
    }
}

