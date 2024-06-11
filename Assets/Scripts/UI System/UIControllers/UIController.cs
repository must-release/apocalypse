using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIEnums;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    public enum STATE { TITLE, CONTROL, STORY, EMPTY, LOADING, SAVE, LOAD, PAUSE }
    public BASEUI CurrentUI { get; private set; } // Used to check if it is save or load UI

    private IUIContoller curUIController; // UIController using right now
    private Stack<IUIContoller> savedUIs; // UIs which will be used again

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
        curUIController = TitleUIController.Instance;
        CurrentUI = BASEUI.TITLE;
        curUIController.StartUI();

        savedUIs = new Stack<IUIContoller>();
    }

    // Change Base UI.
    public void ChangeUI(BASEUI ui)
    {
        curUIController.EndUI();

        switch (ui)
        {
            case BASEUI.TITLE:
                curUIController = TitleUIController.Instance;
                break;
            case BASEUI.STORY:
                curUIController = StoryUIController.Instance;
                break;
            case BASEUI.CONTROL:
                curUIController = ControlUIState.Instance;
                break;
            case BASEUI.LOADING:
                // When loading, exit every previous UIs
                while (savedUIs.Count > 0)
                {
                    savedUIs.Pop().EndUI();
                }
                curUIController = LoadingUIState.Instance;
                break;
        }

        CurrentUI = ui;
        curUIController.StartUI();
    }

    // Turn On Sub UI
    public void TurnOnSubUI(SUBUI subUI)
    {
        switch (subUI)
        {
            case SUBUI.CHOICE:
                StoryUIController.Instance.ShowChoice();
                break;
        }
    }


    private void Update()
    {
        curUIController.UpdateUI();
    }

    public void Move(float move) { curUIController.Move(move); }
    public void Stop() { curUIController.Stop(); }
    public void Attack() { curUIController.Attack(); }
    public void Submit() { curUIController.Submit(); }
    public void Cancel() { curUIController.Cancel(); }
}

public interface IUIContoller
{
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