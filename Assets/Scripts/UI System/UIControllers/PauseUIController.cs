using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIEnums;

/* Part of UIController which manages Pause UI logic */

public class PauseUIController : MonoBehaviour, IUIContoller
{
    /****** Private fields ******/
    private string pauseUIName = "Pause UI";
    private string buttonsName = "Buttons";
    private Transform pauseUI;
    private List<Button> buttonList = new List<Button>();


    /****** Single tone instance ******/
    public static PauseUIController Instance;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Pause UI object
            pauseUI = transform.Find(pauseUIName);
            if (pauseUI == null)
            {
                Debug.Log("pause UI Initialization Error");
                return;
            }

            Transform buttons = pauseUI.Find(buttonsName);
            for (int i = 0; i < buttons.childCount; i++)
            {
                // Add ith button to the list
                buttonList.Add(buttons.GetChild(i).GetComponent<Button>());
            }

            // Add event listener to buttons
            buttonList[0].onClick.AddListener(Cancel);
            buttonList[1].onClick.AddListener(OnSaveButtonClick);
            buttonList[2].onClick.AddListener(OnLoadButtonClick);
            buttonList[3].onClick.AddListener(OnPreferenceButtonClick);
            buttonList[4].onClick.AddListener(OnTitleButtonClick);
        }
    }

    /****** Methods ******/

    // Enter Pause UI
    public void StartUI()
    {
        // Stop time
        Time.timeScale = 0f;

        // Active Pause UI object
        pauseUI.gameObject.SetActive(true);
    }


    // Update Pause UI
    public void UpdateUI()
    {

    }

    // Exit Pause UI
    public void EndUI()
    {
        // Inactive Pause UI object
        pauseUI.gameObject.SetActive(false);

        // start time
        Time.timeScale = 1f;
    }

    // Cancel Pause UI. Return to previous UI
    public void Cancel()
    {
        UIController.Instance.TurnSubUIOff(SUBUI.PAUSE);
    }

    // Open Save UI
    private void OnSaveButtonClick()
    {
        UIController.Instance.TurnSubUIOn(SUBUI.SAVE);
    }

    // Open Load UI
    private void OnLoadButtonClick()
    {
        UIController.Instance.TurnSubUIOn(SUBUI.LOAD);
    }

    // Open Preference UI
    private void OnPreferenceButtonClick()
    {
        UIController.Instance.TurnSubUIOn(SUBUI.PREFERENCE);
    }

    // Go back to title scene
    private void OnTitleButtonClick()
    {
        GameSceneController.Instance.GoTitle();
    }
}
