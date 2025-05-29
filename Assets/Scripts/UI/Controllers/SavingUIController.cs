using UnityEngine;

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

    public SubUI UIType => SubUI.Saving;


    /****** Private Members ******/

    public void Awake()
    {

    }

    public void Start()
    {

    }
}

