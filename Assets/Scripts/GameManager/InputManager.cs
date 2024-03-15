using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; } 
    public enum STATE { PREVIOUS, TITLE, CONTROL, STORY, LOADING, SAVE, LOAD }
    public STATE SaveOrLoad { get; private set; } // Used to check if it is save or load UI

    private IUIState currentUI; // UI using right now
    private Stack<IUIState> savedUIs; // UIs which whill be used again

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

        savedUIs = new Stack<IUIState>();
    }

    // Change UI state.
    public void ChangeState(STATE state, bool endPrevious)
    {
        // If endPrevious is false, save previous UI
        if (endPrevious)
            currentUI.EndUI();
        else
            savedUIs.Push(currentUI);

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
            case STATE.LOADING:
                currentUI = LoadingUIState.Instance;
                break;
            case STATE.SAVE:
                SaveOrLoad = STATE.SAVE;
                currentUI = SaveLoadUIState.Instance;
                break;
            case STATE.LOAD:
                SaveOrLoad = STATE.LOAD;
                currentUI = SaveLoadUIState.Instance;
                break;
            case STATE.PREVIOUS:
                if (savedUIs.Count > 0)
                    currentUI = savedUIs.Pop();
                else
                    Debug.Log("Empty UI Stack");

                // Previous state has already started, thus it doesn't have to be started again
                return;
        }

        currentUI.StartUI();
    }

    private void Update()
    {
        currentUI.UpdateUI();

        if (InputHandler.Instance.Move != 0) { Move(InputHandler.Instance.Move);}
        else { Stop(); }

        if (InputHandler.Instance.Attack) { Attack(); }

        if (InputHandler.Instance.Submit) { Submit(); }

        if (InputHandler.Instance.Cancel) { Cancel(); }
    }

    public void Move(float move) { currentUI.Move(move);}
    public void Stop() { currentUI.Stop(); }
    public void Attack() { currentUI.Attack(); }
    public void Submit() { currentUI.Submit(); }
    public void Cancel() { currentUI.Cancel(); }
}

