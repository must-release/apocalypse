﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/* Part of InputManager which manages Title UI logic */

public class TitleUIController : MonoBehaviour, IUIContoller
{
	public static TitleUIController Instance;
    private string titleUIName = "Title UI";
	private string buttonsName = "Buttons";
	private Transform titleUI;
	private List<Button> buttonList = new List<Button>();

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
            buttonList[0].onClick.AddListener(onContinueGameClick);
            buttonList[1].onClick.AddListener(OnNewGameClick);
            buttonList[2].onClick.AddListener(OnLoadGameClick);
            buttonList[3].onClick.AddListener(OnSettingsClick);
        }
    }


    // Enter Title UI state
    public void StartUI()
	{
		// Active Title UI object
        titleUI.gameObject.SetActive(true);
    }

    // Exit Title UI state
	public void EndUI()
	{
        // Inactive Title UI object
        titleUI.gameObject.SetActive(false);
	}

    // Load most recent saved data
	private void onContinueGameClick()
	{

        // Load most recent saved Data
        DataManager.Instance.LoadContinueData();

        // Start loading recent data
        //GameSceneController.Instance.LoadGameScene()
	}

	// Start new game
	private void OnNewGameClick()
	{
        // Generate new game event stream
        GameEventProducer.Instance.GenerateNewGameEvent();
    }

    // Load saved game
    private void OnLoadGameClick()
    {
        //UIController.Instance.ChangeState(UIController.STATE.LOAD, false);
    }

	private void OnSettingsClick()
	{
        Debug.Log("Settings");
    }

    public UIController.STATE GetState()
    {
        return UIController.STATE.TITLE;
    }

    public void UpdateUI() { return; }
    public void Move(float move) { return; }
    public void Attack() { return; }
    public void Submit() { return; }
    public void Stop() { return; }
    public void Cancel() { return; }
}