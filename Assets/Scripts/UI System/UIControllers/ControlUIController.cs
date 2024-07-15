using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlUIController : MonoBehaviour, IUIContoller
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

    /****** Methods ******/

    // Enter Control UI state
    public void StartUI()
    {
        // Active Story UI object
        controlUI.gameObject.SetActive(true);
<<<<<<< HEAD

        // Get current player data
        UserData playerData = PlayerManager.Instance.PlayerData;

        // Set Control UI
        SetControlUIs(playerData);
=======
>>>>>>> origin/minjung
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
<<<<<<< HEAD

    private void SetControlUIs(UserData playerData)
    {

    }
=======
>>>>>>> origin/minjung
}

