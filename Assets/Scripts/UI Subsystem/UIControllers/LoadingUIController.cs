using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UIEnums;

/* Part of UIController which manages Title UI logic */

public class LoadingUIController : MonoBehaviour, IUIController<BaseUI>
{
    /****** Public Members ******/

    public void EnterUI()
    {
        gameObject.SetActive(true);
    }

    public void UpdateUI()
    {

    }

    public void ExitUI()
    {
        gameObject.SetActive(false);
    }

    public void Cancel() { return; }

    public BaseUI GetUIType() { return BaseUI.Loading; }
    

    /****** Private fields ******/

    private void Awake()
    {

    }

    public void Start()
    {

    }
}