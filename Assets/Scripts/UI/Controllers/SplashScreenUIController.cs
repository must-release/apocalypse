using UnityEngine;

public class SplashScreenUIController : MonoBehaviour, IUIController<BaseUI>
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

    public BaseUI UIType => BaseUI.SplashScreen;


    /****** Private Members ******/

    public void Awake()
    {

    }

    public void Start()
    {

    }
}