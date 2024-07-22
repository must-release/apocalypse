using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIEnums;

/* Part of UIController which manages Title UI logic */

public class TitleUIController : MonoBehaviour, IUIContoller
{
    /****** Private fields ******/
    private string titleUIName = "Title UI";
	private string buttonsName = "Buttons";
	private Transform titleUI;
	private List<Button> buttonList = new List<Button>();


    /****** Single tone instance ******/
    public static TitleUIController Instance;


    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;

            // Find Title UI object
            titleUI = transform.Find(titleUIName);
            if (titleUI == null)
            {
                Debug.Log("Title UI Initialization Error");
                return;
            }

            Transform buttons = titleUI.Find(buttonsName);
            for (int i = 0; i < buttons.childCount; i++)
            {
                // Add ith button to the list
                buttonList.Add(buttons.GetChild(i).GetComponent<Button>());
            }

            // Add event listener to buttons
            buttonList[0].onClick.AddListener(OnContinueGameClick);
            buttonList[1].onClick.AddListener(OnNewGameClick);
            buttonList[2].onClick.AddListener(OnLoadGameClick);
            buttonList[3].onClick.AddListener(OnPreferenceClick);
        }
    }

    /****** Methods ******/

    // Enter Title UI
    public void StartUI()
	{
		// Active Title UI object
        titleUI.gameObject.SetActive(true);
    }


    // Update Title UI
    public void UpdateUI()
    {

    }

    // Exit Title UI
    public void EndUI()
	{
        // Inactive Title UI object
        titleUI.gameObject.SetActive(false);
	}

	private void OnContinueGameClick()
	{
        // Generate Load game event stream, but load most recent saved data
        GameEventProducer.Instance.GenerateLoadGameEventStream();
	}

	private void OnNewGameClick()
	{
        // Generate new game event stream
        GameEventProducer.Instance.GenerateNewGameEventStream();
    }

    private void OnLoadGameClick()
    {
        // Turn Load UI On
        UIController.Instance.TurnSubUIOn(SUBUI.LOAD);
    }

	private void OnPreferenceClick()
	{
        // Turn Preference UI On
        UIController.Instance.TurnSubUIOn(SUBUI.PREFERENCE);
    }

    public void Cancel() { return; }
}