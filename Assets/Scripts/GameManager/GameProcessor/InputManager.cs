using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; } 
    public enum STATE { TITLE, CONTROL, STORY }

    private IUIState currentUI;

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
        currentUI = TitleUIState.Instance;
        currentUI.StartUI();
    }

    // Change UI state
    public void ChangeState(STATE state)
    {
        currentUI.EndUI();

        switch (state)
        {
            case STATE.TITLE:
                currentUI = TitleUIState.Instance;
                break;
            case STATE.STORY:
                currentUI = StoryUIState.Instance;
                break;
            case STATE.CONTROL:
                currentUI = ControlUIState.Instance;
                break;
        }
        currentUI.StartUI();
    }

    private void Update()
    {
        if (InputHandler.Instance.Move != 0) { Move(InputHandler.Instance.Move);}
        else { Stop(); }

        if (InputHandler.Instance.Attack) { Attack(); }

        if(InputHandler.Instance.Submit) { Submit(); }
    }

    public void Move(float move) { currentUI.Move(move);}
    public void Stop() { currentUI.Stop(); }

    public void Attack() { currentUI.Attack(); }

    public void Submit() { currentUI.Submit(); }
}

