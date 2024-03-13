using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlUIState : MonoBehaviour, IUIState
{
    /****** Private fields ******/
    private string controlUIName = "Control UI";
    private static Transform controlUI;


    /****** Single tone instance ******/
    public static ControlUIState Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Title UI object
            controlUI = FindObjectOfType<Canvas>().transform.Find(controlUIName);
            if (controlUI == null)
            {
                Debug.Log("Control UI Initialization Error");
                return;
            }
        }
    }

    /****** Methods ******/

    // Enter Control UI state
    public void StartUI()
    {
        // Active Story UI object
        controlUI.gameObject.SetActive(true);
    }

    // Exit Control UI state
    public void EndUI()
    {
        // Inactive Control UI object
        controlUI.gameObject.SetActive(false);
    }

    public void Cancel()
    {

    }
    public void UpdateUI() { return; }
    public void Attack() { return; }
    public void Submit() { return; }
    public void Move(float move) { return; }
    public void Stop() { return; }
}

