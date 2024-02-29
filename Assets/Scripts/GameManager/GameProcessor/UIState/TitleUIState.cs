using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/* Part of InputManager which manages Title UI logic */

public class TitleUIState : MonoBehaviour, IUIState
{
	private string titleUIName = "Title UI";
	private string buttonsName = "Buttons";
	private static Transform titleUI;
	private static List<Button> buttonList = new List<Button>();

	// UI Initialization
	public void StartUI()
	{
		// Operate only once
		if(titleUI == null)
		{
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

		// In case of console, select first button
        if (IUIState.isConsole)
            buttonList[0].Select();

		// Active Title UI object
        titleUI.gameObject.SetActive(true);
    }

	public void EndUI()
	{
        // Inactive Title UI object
        titleUI.gameObject.SetActive(false);
	}

	public void Move(float move)
	{
		return;
	}

	private void onContinueGameClick()
	{
		Debug.Log("Continue Game");
	}

	// Start new game
	private void OnNewGameClick()
	{
		/* Start stage loading */
		GameManager.Instance.PlayerData = DataManager.Instance.CreateUserData(); // Create new game data
		StageManager.Instance.LoadStage(); // Load stage resources


		/* Play prologue story event */
		StoryEvent prologueStory = DataManager.Instance.CreatePrologueStory(); // Create prologue story event
        EventManager.Instance.PlayEvent(prologueStory); // Play prologue story event
    }

    private void OnLoadGameClick()
    {
        Debug.Log("Load Game");
    }

	private void OnSettingsClick()
	{
        Debug.Log("Settings");
    }
}