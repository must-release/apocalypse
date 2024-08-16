using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UIEnums;

/* Part of UIController which manages Title UI logic */

public class LoadingUIController : MonoBehaviour, IUIController
{
    /****** Private fields ******/
    private string loadingUIName = "Loading UI";
    private Transform loadingUI;


    /****** Single tone instance ******/
    public static LoadingUIController Instance;

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

    public void Start()
    {
        // Add current UI controller
        UIController.Instance.AddUIController(BASEUI.LOADING, Instance);
    }

    /****** UI Methods ******/

    // Enter Loading UI
    public void StartUI()
    {
        // Active Loading UI object
        loadingUI.gameObject.SetActive(true);
    }


    // Update Loading UI
    public void UpdateUI()
    {

    }

    // Exit Loading UI
    public void EndUI()
    {
        // Inactive Loading UI object
        loadingUI.gameObject.SetActive(false);
    }

    public void Cancel() { return; }
}