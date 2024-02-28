using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TitleUIState : MonoBehaviour, IUIState
{
	private string titleUIName = "Title UI";
	private string buttonsName = "Buttons";
	private static Transform titleUI;
	private static List<Button> buttonList = new List<Button>();

	// UI Initialization
	public void StartUI()
	{
		if(titleUI == null)
		{
            titleUI = FindObjectOfType<Canvas>().transform.Find(titleUIName);
			if (titleUI == null)
			{
                Debug.Log("Title UI Initialization Error");
				return;
            }

			Transform buttons = titleUI.Find(buttonsName);
            for (int i = 0; i < buttons.childCount; i++)
            {
                // add ith button to the list
                buttonList.Add(buttons.GetChild(i).GetComponent<Button>());
            }

			// add event listener to buttons
			buttonList[0].onClick.AddListener(onContinueGameClick);
            buttonList[1].onClick.AddListener(OnNewGameClick);
            buttonList[2].onClick.AddListener(OnLoadGameClick);
            buttonList[3].onClick.AddListener(OnSettingsClick);
        }

        if (IUIState.isConsole)
            buttonList[0].Select();

        titleUI.gameObject.SetActive(true);
    }

	public void EndUI()
	{
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

	private void OnNewGameClick()
	{
        // start stage loading
        UserData startData = new UserData(0, 0, null, 0);
		GameManager.Instance.PlayerData = startData;
		StageManager.Instance.LoadStage();

		// play prologue story event
		StoryEvent startStory = ScriptableObject.CreateInstance<StoryEvent>();
		startStory.stageNum = 0;
		startStory.storyNum = 0;
        EventManager.Instance.PlayEvent(startStory);
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