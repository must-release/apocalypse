using UnityEngine;
using System.Collections;

/* Part of UIController which manages Saving UI logic */

public class SavingUIController : MonoBehaviour, IUIContoller
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

    /****** Methods ******/

    // Enter Saving UI state
    public void StartUI()
    {
        // Activate Saving UI object
        savingUI.gameObject.SetActive(true);
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
