using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlUIController : MonoBehaviour, IUIController<BaseUI>
{
    /****** Public Members ******/

    public void EnterUI()
    {
        gameObject.SetActive(true);
        SetControlUI();
    }

    public void UpdateUI()
    {

    }

    public void ExitUI()
    {
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        // Change to Pause UI
        //UIController.Instance.ChangeState(UIController.STATE.PAUSE, false);
    }

    public BaseUI UIType => BaseUI.Control;

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

