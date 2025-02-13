using UnityEngine;
using System.Collections;
using UIEnums;

public class SavingUIController : MonoBehaviour, IUIController<SubUI>
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

    public void Cancel()
    {

    }

    public SubUI GetUIType() { return SubUI.Saving; }


    /****** Private Members ******/

    public void Awake()
    {

    }

    public void Start()
    {

    }
}

