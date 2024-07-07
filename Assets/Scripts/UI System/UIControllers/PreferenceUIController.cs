using UnityEngine;
using System.Collections;

/* Part of UIController which manages Preference UI logic */

public class PreferenceUIController : MonoBehaviour, IUIContoller
{

    /****** Private fields ******/
    private string preferenceUIName = "Preference UI";
    private Transform preferenceUI;


    /****** Single tone instance ******/
    public static PreferenceUIController Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find cutscene UI object
            preferenceUI = transform.Find(preferenceUIName);
            if (preferenceUI == null)
            {
                Debug.Log("Preference UI Initialization Error");
                return;
            }
        }
    }

    /****** Methods ******/

    // Enter Preference UI state
    public void StartUI()
    {
        // Activate preference UI object
        preferenceUI.gameObject.SetActive(true);
    }

    // Exit Preference UI state
    public void EndUI()
    {
        // Inactivate preference UI object
        preferenceUI.gameObject.SetActive(false);
    }

    public void Cancel()
    {

    }

    public void UpdateUI() { return; }
}

