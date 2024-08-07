using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UIEnums;

/* Part of UIController which manages Pause UI logic */

public class PauseUIController : MonoBehaviour, IUIContoller
{
    /****** Private fields ******/
    private string pauseUIName = "Pause UI";
    private string confirmBoxName = "Confirm Box";
    private string confirmButtonName = "Confirm Button";
    private string cancelButtonName = "Cancel Button";
    private string buttonsName = "Buttons";
    private Transform pauseUI;
    private Transform confirmBox;
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
            confirmBox = pauseUI.Find(confirmBoxName);
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
            confirmBox.Find(confirmButtonName).GetComponent<Button>().onClick.AddListener(ReturnToTitle);
            confirmBox.Find(cancelButtonName).GetComponent<Button>().onClick.AddListener(Cancel);
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
        // Inactive Pause UI objects
        EventSystem.current.SetSelectedGameObject(null);
        confirmBox.gameObject.SetActive(false);
        pauseUI.gameObject.SetActive(false);

        // start time
        Time.timeScale = 1f;
    }

    // Cancel Pause UI. Return to previous UI
    public void Cancel()
    {
        if(confirmBox.gameObject.activeInHierarchy) // When confirm box is on
        {
            confirmBox.gameObject.SetActive(false);
        }
        else
        {
            UIController.Instance.TurnSubUIOff(SUBUI.PAUSE);
        }
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
        EventSystem.current.SetSelectedGameObject(null);
        confirmBox.gameObject.SetActive(true);
    }
    private void ReturnToTitle()
    {
        UIController.Instance.TurnEverySubUIOff();
        GameEventProducer.Instance.GenerateChangeSceneEventStream(SceneEnums.SCENE.TITLE);
    }
}
