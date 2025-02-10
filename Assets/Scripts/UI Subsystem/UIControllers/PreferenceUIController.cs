using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIEnums;

public class PreferenceUIController : MonoBehaviour, IUIController<SubUI>
{
    /****** Public Members ******/

    public void StartUI()
    {
        gameObject.SetActive(true);
    }

    public void UpdateUI()
    {

    }

    public void Cancel()
    {
        UIController.Instance.TurnSubUIOff( GetUIType() );
    }

    public void EndUI()
    {
        gameObject.SetActive(false);
    }

    public SubUI GetUIType() { return SubUI.Preference; }

    
    /****** Private Members ******/

    private Transform _preferenceScroll;
    private Transform _content;
    private List<Button> _contentButtons;

    private void Awake()
    {
        // Find Preference Scroll object
        _preferenceScroll = transform.Find("PreferenceScroll");
        if ( null == _preferenceScroll )
        {
            Debug.LogError("Preference Scroll Initialization Error");
            return;
        }

        // Find Content through Viewport
        Transform viewport = _preferenceScroll.Find("Viewport");
        if ( null == viewport)
        {
            Debug.LogError("Viewport Initialization Error");
            return;
        }

        _content = viewport.Find("Content");
        if ( null == _content)
        {
            Debug.LogError("Content Initialization Error");
            return;
        }

        // Find all buttons under Content
        _contentButtons = new List<Button>();
        Button[] buttons = _content.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            _contentButtons.Add(button);
        }

        // Add event listeners to buttons by order
        if (_contentButtons.Count > 0) _contentButtons[0].onClick.AddListener(OnWindowScreenButton);
        if (_contentButtons.Count > 1) _contentButtons[1].onClick.AddListener(OnFullScreenButton);
        if (_contentButtons.Count > 2) _contentButtons[2].onClick.AddListener(OnMovetoKeySettingsButton);

        // Find and set up Reset and Confirm buttons in Preference UI
        Button resetButton = transform.Find("Buttons/Reset Button")?.GetComponent<Button>();
        if ( null != resetButton)
        {
            resetButton.onClick.AddListener(OnResetButton);
        }

        Button confirmButton = transform.Find("Buttons/Confirm Button")?.GetComponent<Button>();
        if ( null != confirmButton )
        {
            confirmButton.onClick.AddListener(OnConfirmButton);
        }
    }

    private void Start()
    {

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
        UIController.Instance.TurnSubUIOn(SubUI.KeySettings);
    }
}
