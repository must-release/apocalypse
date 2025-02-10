using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIEnums;

public class ControlUIController : MonoBehaviour, IUIController<BaseUI>
{
    /****** Public Members ******/

    public void StartUI()
    {
        gameObject.SetActive(true);
        SetControlUI();
    }

    public void UpdateUI()
    {

    }

    public void EndUI()
    {
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        // Change to Pause UI
        //UIController.Instance.ChangeState(UIController.STATE.PAUSE, false);
    }

    public BaseUI GetUIType() { return BaseUI.Control; }

    /****** Private Members ******/

    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void SetControlUI()
    {

    }
}

