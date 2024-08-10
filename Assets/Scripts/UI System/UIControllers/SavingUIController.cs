using UnityEngine;
using System.Collections;
using UIEnums;

/* Part of UIController which manages Saving UI logic */

public class SavingUIController : MonoBehaviour, IUIController
{

    /****** Private fields ******/
    private string savingUIName = "Saving UI";
    private Transform savingUI;


    /****** Single tone instance ******/
    public static SavingUIController Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find cutscene UI object
            savingUI = transform.Find(savingUIName);
            if (savingUI == null)
            {
                Debug.Log("Saving UI Initialization Error");
                return;
            }
        }
    }

    public void Start()
    {
        // Add current UI controller
        UIController.Instance.AddUIController(SUBUI.SAVING, Instance);
    }

    /****** Methods ******/

    // Enter Saving UI state
    public void StartUI()
    {
        // Activate Saving UI object
        savingUI.gameObject.SetActive(true);
    }


    // Update Saving UI
    public void UpdateUI()
    {

    }

    // Exit Saving UI state
    public void EndUI()
    {
        // Inactivate Saving UI object
        savingUI.gameObject.SetActive(false);
    }

    public void Cancel()
    {
    }
}

