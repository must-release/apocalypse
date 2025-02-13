using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIEnums;

public class TitleUIController : MonoBehaviour, IUIController<BaseUI>
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

    public BaseUI GetUIType() { return BaseUI.Title; }


    /****** Private Members ******/
    
	private const string _ButtonsName = "Buttons";
	private List<Button> buttonList;

    private void Awake()
    {
        buttonList  = new List<Button>();

        Transform buttons = transform.Find(_ButtonsName);
        for (int i = 0; i < buttons.childCount; i++)
        {
            buttonList.Add(buttons.GetChild(i).GetComponent<Button>());
        }

        buttonList[0].onClick.AddListener(OnContinueGameClick);
        buttonList[1].onClick.AddListener(OnNewGameClick);
        buttonList[2].onClick.AddListener(OnLoadGameClick);
        buttonList[3].onClick.AddListener(OnPreferenceClick);
    }

    private void Start()
    {

    }

	private void OnContinueGameClick()
	{
        // Generate Load game event stream, but load most recent saved data
        GameEventProducer.Instance.GenerateLoadGameEventStream();
	}

	private void OnNewGameClick()
	{
        // Generate new game event stream
        GameEventProducer.Instance.GenerateNewGameEventStream();
    }

    private void OnLoadGameClick()
    {
        // Turn Load UI On
        UIController.Instance.TurnSubUIOn(SubUI.Load);
    }

	private void OnPreferenceClick()
	{
        // Turn Preference UI On
        UIController.Instance.TurnSubUIOn(SubUI.Preference);
    }
}