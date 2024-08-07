using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIEnums;
using System;

/* Part of UIController which manages Preference UI logic */

public class PreferenceUIController : MonoBehaviour, IUIContoller
{
    /****** Private fields ******/
    private string preferenceUIName = "Preference UI";
    private Transform preferenceUI;
    private Transform preferenceScroll;
    private Transform content;
    private List<Button> contentButtons = new List<Button>();

    /****** Single tone instance ******/
    public static PreferenceUIController Instance;

    public void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;

        //     // Find Preference UI object
        //     preferenceUI = transform.Find(preferenceUIName);
        //     if (preferenceUI == null)
        //     {
        //         Debug.LogError("Preference UI Initialization Error");
        //         return;
        //     }

        //     // Find Preference Scroll object
        //     preferenceScroll = preferenceUI.Find("PreferenceScroll");
        //     if (preferenceScroll == null)
        //     {
        //         Debug.LogError("Preference Scroll Initialization Error");
        //         return;
        //     }

        //     // Find Content through Viewport
        //     Transform viewport = preferenceScroll.Find("Viewport");
        //     if (viewport == null)
        //     {
        //         Debug.LogError("Viewport Initialization Error");
        //         return;
        //     }

        //     content = viewport.Find("Content");
        //     if (content == null)
        //     {
        //         Debug.LogError("Content Initialization Error");
        //         return;
        //     }

        //     // Find all buttons under Content
        //     Button[] buttons = content.GetComponentsInChildren<Button>();
        //     foreach (Button button in buttons)
        //     {
        //         contentButtons.Add(button);
        //     }

        //     // Add event listeners to buttons by order
        //     if (contentButtons.Count > 0) contentButtons[0].onClick.AddListener(OnWindowScreenButton);
        //     if (contentButtons.Count > 1) contentButtons[1].onClick.AddListener(OnFullScreenButton);
        //     if (contentButtons.Count > 2) contentButtons[2].onClick.AddListener(OnMovetoKeySettingsButton);
        //     // Add more if necessary

        //     // Find and set up Reset and Confirm buttons in Preference UI
        //     Button resetButton = preferenceUI.Find("Buttons/Reset Button")?.GetComponent<Button>();
        //     if (resetButton != null)
        //     {
        //         resetButton.onClick.AddListener(OnResetButton);
        //     }

        //     Button confirmButton = preferenceUI.Find("Buttons/Confirm Button")?.GetComponent<Button>();
        //     if (confirmButton != null)
        //     {
        //         confirmButton.onClick.AddListener(OnConfirmButton);
        //     }
        // }
    }

    public void Start()
    {

    }

    /****** Methods ******/

    private void Update()
    {
    }

    // Enter Preference UI state
    public void StartUI()
    {
        if (preferenceUI != null)
        {
            preferenceUI.gameObject.SetActive(true);
        }
    }


    // Update Preference UI
    public void UpdateUI()
    {

    }

    // Exit Preference UI state
    public void EndUI()
    {
        if (preferenceUI != null)
        {
            preferenceUI.gameObject.SetActive(false);
        }
    }

    private void OnWindowScreenButton()
    {
        Debug.Log("Window Screen Button Clicked");
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    private void OnFullScreenButton()
    {
        Debug.Log("Full Screen Button Clicked");
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    private void OnConfirmButton()
    {
        Debug.Log("Confirm Button Clicked");
    }

    private void OnResetButton()
    {
        Debug.Log("Reset Button Clicked");
    }

    private void OnMovetoKeySettingsButton()
    {
        Debug.Log("Move to Key Settings Button Clicked");
        UIController.Instance.TurnSubUIOn(SUBUI.KEYSETTINGS);
    }

    public void Cancel()
    {
        Debug.Log("Cancel");
        UIController.Instance.TurnSubUIOff(SUBUI.PREFERENCE);
    }

    private void Return()
    {
    }
}
