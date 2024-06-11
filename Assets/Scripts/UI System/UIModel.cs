using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIEnums;

public class UIModel : MonoBehaviour
{
    public static UIModel Instance { get; private set; }

    private BASEUI currentUI; // UI using right now
    private Stack<BASEUI> savedUIs; // UIs which will be used again

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
        currentUI = BASEUI.TITLE;

        savedUIs = new Stack<BASEUI>();
    }
}

