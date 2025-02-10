using UIEnums;
using UnityEngine;

public class SplashScreenUIController : MonoBehaviour, IUIController<BaseUI>
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

    public void Cancel() { return; }

    public BaseUI GetUIType() { return BaseUI.SplashScreen; }


    /****** Private Members ******/

    public void Awake()
    {

    }

    public void Start()
    {

    }
}