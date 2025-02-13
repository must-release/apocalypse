using UnityEngine;
using UIEnums;
using System.Collections.Generic;

public class CutsceneUIController : MonoBehaviour, IUIController<BaseUI>
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

    public BaseUI GetUIType() { return BaseUI.Cutscene; }


    /****** Private Members ******/

    private void Awake()
    {

    }

    private void Start()
    {

    }
}

