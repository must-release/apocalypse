using UnityEngine;
using System.Collections.Generic;

public class CutsceneUIController : MonoBehaviour, IUIView<BaseUI>
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

    public BaseUI UIType => BaseUI.Cutscene;
    

    /****** Private Members ******/

    private void Awake()
    {

    }

    private void Start()
    {

    }
}

