using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; } 
    public enum STATE { TITLE, CONTROL, STORY }

    IUIState currentUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // initalize current UI to title UI
            currentUI = GetComponent<TitleUIState>();
            currentUI.StartUI();
        }
    }

    // Change UI state
    public void ChangeState(STATE state)
    {
        currentUI.EndUI();

        switch (state)
        {
            case STATE.TITLE:
                currentUI = GetComponent<TitleUIState>();
                break;
            case STATE.STORY:
                currentUI = GetComponent<StoryUIState>();
                break;
        }

        currentUI.StartUI();
    }

    private void Update()
    {
        if (InputHandler.Instance.Move != 0)
        {
            Move(InputHandler.Instance.Move);
        }
    }

    public void Move(float move)
    {
        currentUI.Move(move);
    }

}

