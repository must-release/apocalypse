using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    public enum STATE { TITLE, CONTROL, STORY, EMPTY, LOADING, SAVE, LOAD, PAUSE }
    public STATE CurrentState { get; private set; } // Used to check if it is save or load UI

    private IUIContollerState currentUI; // UI using right now
    private Stack<IUIContollerState> savedUIs; // UIs which will be used again

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
        CurrentState = STATE.TITLE;
        currentUI.StartUI();

        savedUIs = new Stack<IUIContollerState>();
    }

    // Change UI state.
    public void ChangeState(STATE state, bool endPrevious)
    {
        // If endPrevious is false, save previous UI
        if (endPrevious)
        {
            currentUI.EndUI();
        }
        else
        {
            savedUIs.Push(currentUI);
        }

        switch (state)
        {
            case STATE.TITLE:
                // When going back to title, exit every previous UIs
                while (savedUIs.Count > 0)
                {
                    savedUIs.Pop().EndUI();
                }
                currentUI = TitleUIState.Instance;
                break;
            case STATE.STORY:
                currentUI = StoryUIState.Instance;
                break;
            case STATE.CONTROL:
                currentUI = ControlUIState.Instance;
                break;
            case STATE.LOADING:
                // When loading, exit every previous UIs
                while (savedUIs.Count > 0)
                {
                    savedUIs.Pop().EndUI();
                }
                currentUI = LoadingUIState.Instance;
                break;
            case STATE.EMPTY:
                currentUI = EmptyUIState.Instance;
                break;
            case STATE.SAVE:
                currentUI = SaveLoadUIState.Instance;
                break;
            case STATE.LOAD:
                currentUI = SaveLoadUIState.Instance;
                break;
            case STATE.PAUSE:
                currentUI = PauseUIState.Instance;
                break;
        }

        CurrentState = state;
        currentUI.StartUI();
    }

    // Change to previous UI state
    public void ChangeToPreviousState()
    {
        if (savedUIs.Count > 0)
        {
            STATE nextState = savedUIs.Pop().GetState();
            ChangeState(nextState, true);
        }
        else
            Debug.Log("Empty UI Stack");

        return;
    }

    // Refresh current UI
    public void RefreshState()
    {
        currentUI.StartUI();
    }

    private void Update()
    {
        currentUI.UpdateUI();
    }

    public void Move(float move) { currentUI.Move(move); }
    public void Stop() { currentUI.Stop(); }
    public void Attack() { currentUI.Attack(); }
    public void Submit() { currentUI.Submit(); }
    public void Cancel() { currentUI.Cancel(); }
}

public interface IUIContollerState
{
    public static bool isConsole = false;

    public void StartUI();
    public void UpdateUI();
    public void EndUI();
    public UIController.STATE GetState();
    public void Move(float move);
    public void Stop();
    public void Attack();
    public void Submit();
    public void Cancel();
}