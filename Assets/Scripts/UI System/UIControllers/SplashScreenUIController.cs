using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UIEnums;
using UnityEngine;

/* Part of UIController which manages Title UI logic */

public class SplashScreenUIController : MonoBehaviour, IUIController
{
    /****** Private fields ******/
    private string splashScreenUIName = "Splash Screen UI";
    private Transform splashScreenUI;


    /****** Single tone instance ******/
    public static SplashScreenUIController Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Title UI object
            splashScreenUI = transform.Find(splashScreenUIName);
            if (splashScreenUI == null)
            {
                Debug.Log("Splash Screen UI Initialization Error");
                return;
            }
        }
    }

    public void Start()
    {
        // Add current UI controller
        UIController.Instance.AddUIController(BASEUI.SPLASH_SCREEN, Instance);
    }

    /****** UI Methods ******/

    // Enter splash screen UI
    public void StartUI()
    {
        // Active splash screen UI object
        splashScreenUI.gameObject.SetActive(true);
    }


    // Update splash screen UI
    public void UpdateUI()
    {

    }

    // Exit splash screen UI
    public void EndUI()
    {
        // Inactive splash screen UI object
        splashScreenUI.gameObject.SetActive(false);
    }

    public void Cancel() { return; }
}