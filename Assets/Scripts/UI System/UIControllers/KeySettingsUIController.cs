using UnityEngine;
<<<<<<< HEAD
using System.Collections;
=======
using UnityEngine.UI;
using System.Collections.Generic;
using UIEnums;
using System;
>>>>>>> origin/minjung

/* Part of UIController which manages Preference UI logic */

public class KeySettingsUIController : MonoBehaviour, IUIContoller
{

    /****** Private fields ******/
    private string keySettingsUIName = "Key Settings UI";
    private Transform keySettingsUI;


    /****** Single tone instance ******/
    public static KeySettingsUIController Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find cutscene UI object
            keySettingsUI = transform.Find(keySettingsUIName);
            if (keySettingsUI == null)
            {
                Debug.Log("Key Settings UI Initialization Error");
                return;
            }
        }
    }

    /****** Methods ******/

<<<<<<< HEAD
=======
    //private void Update()
    //{
    //    Return();
    //}

>>>>>>> origin/minjung
    // Enter Key Settings UI state
    public void StartUI()
    {
        // Activate key settings UI object
        keySettingsUI.gameObject.SetActive(true);
    }

    // Exit Key Settings UI state
    public void EndUI()
    {
        // Inactivate key settings UI object
        keySettingsUI.gameObject.SetActive(false);
    }

<<<<<<< HEAD
=======
    //public void Return()
    //{
    //    if (Input.GetKeyDown(KeyCode.Backspace))
    //    {
    //        UIController.Instance.TurnSubUIOff(SUBUI.KEYSETTINGS);
    //    }
    //}

>>>>>>> origin/minjung
    public void Cancel()
    {
    }
}

