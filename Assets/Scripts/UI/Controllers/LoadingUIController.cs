using UnityEngine;

/* Part of UIController which manages Title UI logic */

public class LoadingUIController : MonoBehaviour, IUIView<BaseUI>
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

    public BaseUI UIType => BaseUI.Loading;
    

    /****** Private fields ******/

    private void Awake()
    {

    }

    public void Start()
    {

    }
}