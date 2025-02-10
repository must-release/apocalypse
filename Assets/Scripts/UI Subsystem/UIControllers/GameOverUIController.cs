using UnityEngine;
using UIEnums;
using System.Collections.Generic;

public class GameOverUIController : MonoBehaviour, IUIController<BaseUI>
{
    /****** Public Members ******/

    public void StartUI()
    {
        gameObject.SetActive(true);
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

    }

    public BaseUI GetUIType() { return BaseUI.GameOver; }


    /****** Private Members ******/

    private void Awake()
    {

    }

    private void Start()
    {

    }
}

