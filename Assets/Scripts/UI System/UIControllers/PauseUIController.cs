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
            buttonList[0].onClick.AddListener(OnResumeButtonClick);
            buttonList[1].onClick.AddListener(OnSaveButtonClick);
            buttonList[2].onClick.AddListener(OnLoadButtonClick);
            buttonList[3].onClick.AddListener(OnSettingsButtonClick);
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

    // Exit Pause UI
    public void EndUI()
    {
        // Inactive Pause UI object
        pauseUI.gameObject.SetActive(false);

        // start time
        Time.timeScale = 1f;
    }

    public void Cancel()
    {
        // Change to previous state
        //UIController.Instance.ChangeToPreviousState();
    }

    public void OnResumeButtonClick()
    {
        // Change to previous state
        //UIController.Instance.ChangeToPreviousState();
    }

    // Open Save UI
    private void OnSaveButtonClick()
    {
        UIController.Instance.TurnSubUIOn(SUBUI.SAVE);
    }

    private void OnLoadButtonClick()
    {
        // Open Load UI
        //UIController.Instance.ChangeState(UIController.STATE.LOAD, false);
    }

    private void OnSettingsButtonClick()
    {
        Debug.Log("settings click");
    }

    // Go back to title scene
    private void OnTitleButtonClick()
    {
        GameSceneController.Instance.GoTitle();
    }

    public void UpdateUI() { return; }
    public void Attack() { return; }
    public void Submit() { return; }
    public void Move(float move) { return; }
    public void Stop() { return; }
}
