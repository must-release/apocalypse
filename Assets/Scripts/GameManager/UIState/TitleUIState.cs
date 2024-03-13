using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/* Part of InputManager which manages Title UI logic */

public class TitleUIState : MonoBehaviour, IUIState
{
	public static TitleUIState Instance;

	private string titleUIName = "Title UI";
	private string buttonsName = "Buttons";
	private static Transform titleUI;
	private static List<Button> buttonList = new List<Button>();

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;

            // Find Title UI object
            titleUI = FindObjectOfType<Canvas>().transform.Find(titleUIName);
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
            buttonList[0].onClick.AddListener(onContinueGameClick);
            buttonList[1].onClick.AddListener(OnNewGameClick);
            buttonList[2].onClick.AddListener(OnLoadGameClick);
            buttonList[3].onClick.AddListener(OnSettingsClick);
        }
    }


    // Enter Title UI state
    public void StartUI()
	{
		// In case of console, select first button
        if (IUIState.isConsole)
            buttonList[0].Select();

		// Active Title UI object
        titleUI.gameObject.SetActive(true);
    }

    // Exit Title UI state
	public void EndUI()
	{
        // Inactive Title UI object
        titleUI.gameObject.SetActive(false);
	}

	private void onContinueGameClick()
	{
        /* Load most recent saved Data */
        DataManager.Instance.LoadContinueData();

        /* Start stage loading */
        StageManager.Instance.LoadStage();
	}

	// Start new game
	private void OnNewGameClick()
	{

        /* Create new game data */
        DataManager.Instance.CreateUserData();

        /* Start stage loading */
        StageManager.Instance.LoadStage();


    }

    private void OnLoadGameClick()
    {
        Debug.Log("Load Game");
    }

	private void OnSettingsClick()
	{
        Debug.Log("Settings");
    }

    public void UpdateUI() { return; }
    public void Move(float move) { return; }
    public void Attack() { return; }
    public void Submit() { return; }
    public void Stop() { return; }
    public void Cancel() { return; }
}