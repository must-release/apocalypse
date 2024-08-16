using UnityEngine;
using System.Collections.Generic;
using UIEnums;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    private IUIController curUIController; // UIController using right now
    private Dictionary<BASEUI, IUIController> baseUIDictionary;
    private Dictionary<SUBUI, IUIController> subUIDictionary;

    // Story panel clicked info
    private bool isStoryPanelClicked;
    public bool IsStoryPanelClicked
    {
        get 
        {
            bool _clicked = isStoryPanelClicked;
            isStoryPanelClicked = false;
            return _clicked; 
        }
        set { isStoryPanelClicked = value; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            baseUIDictionary = new Dictionary<BASEUI, IUIController>();
            subUIDictionary = new Dictionary<SUBUI, IUIController>();
            isStoryPanelClicked = false;
        }
    }

    // Change Base UI.
    public void ChangeBaseUI(BASEUI baseUI)
    {
        curUIController?.EndUI();

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
            Debug.LogError("Sub UI Mismatch \n" + subUI + '\n' + UIModel.Instance.CurrentSubUI);
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

            // Update Sub UI
            curUIController.UpdateUI();
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
    public void GetCurrentUI(out BASEUI baseUI, out SUBUI subUI)
    {
        baseUI = UIModel.Instance.CurrentBaseUI;
        subUI = UIModel.Instance.CurrentSubUI;
    }

    // Set choice info
    public void SetChoiceInfo(List<string> choiceList)
    {
        UIModel.Instance.ChoiceList = choiceList;
        UIModel.Instance.SelectedChoice = null;
    }

    // Get selected choice
    public string GetSelectedChoice() { return UIModel.Instance.SelectedChoice; }

    // Cancel current UI
    public void CancelCurrentUI() { curUIController.Cancel(); }

    // Add new UIController to dictionary
    public void AddUIController(BASEUI baseUI, IUIController newController)
    {
        baseUIDictionary[baseUI] = newController;
    }
    public void AddUIController(SUBUI subUI, IUIController newController)
    {
        subUIDictionary[subUI] = newController;
    }

    // Set UI controller according to parameter base UI
    private void SetUIController(BASEUI baseUI)
    {
        if (baseUIDictionary.TryGetValue(baseUI, out IUIController controller))
        {
            curUIController = controller;
        }
        else
        {
            Debug.Log("Such base UI does not exists: " + baseUI.ToString());
        }
    }

    // Set UI controller according to parameter sub UI
    private void SetUIController(SUBUI subUI)
    {
        if(subUI == SUBUI.NONE)
        {
            SetUIController(UIModel.Instance.CurrentBaseUI);
        }
        else if (subUIDictionary.TryGetValue(subUI, out IUIController controller))
        {
            curUIController = controller;
        }
        else
        {
            Debug.Log("Such sub UI does not exists: " + subUI.ToString());
        }
    }
}

public interface IUIController
{
    public void StartUI();
    public void UpdateUI();
    public void EndUI();
    public void Cancel();
}