using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/* Part of UIController which manages Title UI logic */

public class SplashScreenUIController : MonoBehaviour, IUIContoller
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

    /****** UI Methods ******/

    // Enter splash screen UI
    public void StartUI()
    {
        // Active splash screen UI object
        splashScreenUI.gameObject.SetActive(true);

        // Stop current thread for 2 seconds. Must be fixed.
        Thread.Sleep(2000);
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