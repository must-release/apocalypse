using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIEnums;
using System.Buffers.Text;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    private IUIContoller curUIController; // UIController using right now

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
        UIModel.Instance.CurrentBaseUI = BASEUI.TITLE;
        SetUIController(BASEUI.TITLE);
        curUIController.StartUI();
    }

    // Change Base UI.
    public void ChangeBaseUI(BASEUI baseUI)
    {
        curUIController.EndUI();

        UIModel.Instance.CurrentBaseUI = baseUI;
        SetUIController(baseUI);

        curUIController.StartUI();
    }

    // Turn Sub UI On
    public void TurnSubUIOn(SUBUI subUI)
    {
        // Stack sub UI
        UIModel.Instance.PushNewSubUI(subUI); 

        // Set current UI Controller to sub UI
        SetUIController(subUI);

        // Start Sub UI
        curUIController.StartUI();
    }

    // Turn Sub UI Off
    public void TurnSubUIOff(SUBUI subUI)
    {
        // Check if it is a right call
        if (UIModel.Instance.CurrentSubUI != subUI || subUI == SUBUI.NONE)
        {
            Debug.Log("Sub UI Mismatch");
            return;
        }

        // End current Sub UI
        curUIController.EndUI();
        UIModel.Instance.PopCurrentSubUI();

        // Check what UI comes next
        if(UIModel.Instance.CurrentSubUI == SUBUI.NONE) // When there is no sub UI left in the stack
        {
            // Set curUIController to Base UI
            SetUIController(UIModel.Instance.CurrentBaseUI);
        }
        else // If there is sub UI left in the stack
        {
            // Set previous sub UI to current UI controller
            SetUIController(UIModel.Instance.CurrentSubUI);

            // Start Sub UI
            curUIController.StartUI();
        }
    }

    // Turn Every Sub UI Off
    public void TurnEverySubUIOff()
    {
        while(UIModel.Instance.CurrentSubUI != SUBUI.NONE)
        {
            // End current sub UI
            curUIController.EndUI();
            UIModel.Instance.PopCurrentSubUI();

            // Set current UI controller to previous sub UI
            SetUIController(UIModel.Instance.CurrentSubUI);
        }
    }

    // Get current UI. Return both base UI and sub UI
    private (BASEUI, SUBUI) GetCurrentUI()
    {
        return (UIModel.Instance.CurrentBaseUI, UIModel.Instance.CurrentSubUI);
    }

    // Set UI controller according to parameter base UI
    private void SetUIController(BASEUI baseUI)
    {
        switch (baseUI)
        {
            case BASEUI.TITLE:
                curUIController = TitleUIController.Instance;
                break;
            case BASEUI.STORY:
                curUIController = StoryUIController.Instance;
                break;
            case BASEUI.CONTROL:
                curUIController = ControlUIController.Instance;
                break;
            case BASEUI.LOADING:
                curUIController = LoadingUIController.Instance;
                break;
        }
    }

    // Set UI controller according to parameter sub UI
    private void SetUIController(SUBUI subUI)
    {
        switch (subUI)
        {
            case SUBUI.NONE:
                SetUIController(UIModel.Instance.CurrentBaseUI);
                break;
            case SUBUI.LOAD:
                curUIController = SaveLoadUIController.Instance;
                break;
            case SUBUI.PREFERENCE:
                curUIController = PreferenceUIController.Instance;
                break;
            case SUBUI.KEYSETTINGS:
                curUIController = KeySettingsUIController.Instance;
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
    public void Move(float move);
    public void Stop();
    public void Attack();
    public void Submit();
    public void Cancel();
}