﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/* Part of InputManager which manages Title UI logic */

public class LoadingUIState : MonoBehaviour, IUIContollerState
{
    public static LoadingUIState Instance;

    private string loadingUIName = "Loading UI";
    private Transform loadingUI;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Title UI object
            loadingUI = transform.Find(loadingUIName);
            if (loadingUI == null)
            {
                Debug.Log("Loading UI Initialization Error");
                return;
            }
        }
    }

    // Enter Loading UI state
    public void StartUI()
    {
        // if Stage asset load is complete, don't start loading
        if (SceneController.Instance.IsStageReady)
        {
            GameEventRouter.Instance.EventOver();
            return;
        }

        // Active Loading UI object
        loadingUI.gameObject.SetActive(true);

        // Disable stage objects
        SceneController.Instance.SetStageObjectsActive(false);
    }


    // Update Loading UI state
    public void UpdateUI()
    {
        // if Stage asset load is complete, end loading
        if (SceneController.Instance.IsStageReady)
        {
            GameEventRouter.Instance.EventOver();
        }
    }


    // Exit Loading UI state
    public void EndUI()
    {
        // Enable stage objects
        SceneController.Instance.SetStageObjectsActive(true);

        // Inactive Title UI object
        loadingUI.gameObject.SetActive(false);
    }

    public UIController.STATE GetState()
    {
        return UIController.STATE.LOADING;
    }

    public void Move(float move) { return; }
    public void Attack() { return; }
    public void Submit() { return; }
    public void Stop() { return; }
    public void Cancel() { return; }
}