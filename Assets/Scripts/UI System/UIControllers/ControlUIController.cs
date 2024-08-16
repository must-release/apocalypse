using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIEnums;

public class ControlUIController : MonoBehaviour, IUIController
{
    /****** Private fields ******/
    private string controlUIName = "Control UI";
    private Transform controlUI;

    /****** Single tone instance ******/
    public static ControlUIController Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Title UI object
            controlUI = transform.Find(controlUIName);
            if (controlUI == null)
            {
                Debug.Log("Control UI Initialization Error");
                return;
            }
        }
    }

    public void Start()
    {
        // Add current UI controller
        UIController.Instance.AddUIController(BASEUI.CONTROL, Instance);
    }

    /****** Methods ******/

    // Enter Control UI state
    public void StartUI()
    {
        // Active Story UI object
        controlUI.gameObject.SetActive(true);

        // Set Control UI according to player data
        SetControlUIs();
    }

    // Update Control UI
    public void UpdateUI()
    {

    }

    // Exit Control UI state
    public void EndUI()
    {
        // Inactive Control UI object
        controlUI.gameObject.SetActive(false);
    }

    public void Cancel()
    {
        // Change to Pause UI
        //UIController.Instance.ChangeState(UIController.STATE.PAUSE, false);
    }

    private void SetControlUIs()
    {

    }
}

