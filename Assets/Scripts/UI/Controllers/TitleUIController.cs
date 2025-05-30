using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TitleUIController : MonoBehaviour, IUIController<BaseUI>
{
    /****** Public Members ******/

    public BaseUI UIType => BaseUI.Title;

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

    public void Cancel() { }


    /****** Private Members ******/
    
	private const string    _ButtonsName            = "Buttons";
	private List<Button>    _buttonList             = new List<Button>();

    private void Awake()
    {
        Transform buttons = transform.Find(_ButtonsName);
        for (int i = 0; i < buttons.childCount; i++)
        {
            _buttonList.Add(buttons.GetChild(i).GetComponent<Button>());
        }

        _buttonList[0].onClick.AddListener(OnContinueGameClick);
        _buttonList[1].onClick.AddListener(OnNewGameClick);
        _buttonList[2].onClick.AddListener(OnLoadGameClick);
        _buttonList[3].onClick.AddListener(OnPreferenceClick);
    }

	private void OnContinueGameClick()
	{
        GameEventManager.Instance.Submit(GameEventFactory.CreateCommonEvent(CommonEventType.Continue));
	}

	private void OnNewGameClick()
	{
        GameEventManager.Instance.Submit(GameEventFactory.CreateCommonEvent(CommonEventType.NewGame));
    }

    private void OnLoadGameClick()
    {
        UIController.Instance.TurnSubUIOn(SubUI.Load);
    }

	private void OnPreferenceClick()
	{
        UIController.Instance.TurnSubUIOn(SubUI.Preference);
    }
}