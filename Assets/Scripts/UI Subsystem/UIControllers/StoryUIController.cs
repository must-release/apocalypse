using UnityEngine;
using UIEnums;

/* Part of UIController which manages Story UI logic */

public class StoryUIController : MonoBehaviour, IUIController<BaseUI>
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

    public BaseUI GetUIType() { return BaseUI.Story; }


    /****** Private Members ******/

    public void Awake()
    {

    }

    public void Start()
    {

    }

}