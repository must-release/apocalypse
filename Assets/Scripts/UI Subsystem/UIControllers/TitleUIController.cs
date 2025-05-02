using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIEnums;
using UnityEngine.AddressableAssets;
using AssetEnums;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TitleUIController : MonoBehaviour, IUIController<BaseUI>, IAsyncLoadObject
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

    public BaseUI UIType => BaseUI.Title;

    public bool IsLoaded => _isLoaded;


    /****** Private Members ******/
    
	private const string    _ButtonsName            = "Buttons";
	private List<Button>    _buttonList             = new List<Button>();
    private GameEventInfo   _continueGameEventInfo  = null;
    private GameEventInfo   _newGameEventInfo       = null;
    private bool            _isLoaded               = false;

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

    private void Start()
    {
        StartCoroutine(AsyncLoadGameEventInfos());
    }

    private IEnumerator AsyncLoadGameEventInfos()
    {
        var continueGameEvent = Addressables.LoadAssetAsync<GameEventInfo>(SequentialEventInfoAsset.Common.ContinueGame);
        yield return continueGameEvent;
        if (continueGameEvent.Status == AsyncOperationStatus.Succeeded)
        {
            _continueGameEventInfo = continueGameEvent.Result;
        }
        else
        {
            Debug.LogError("Failed to load ContinueGame event info.");
        }

        var newGameEvent = Addressables.LoadAssetAsync<GameEventInfo>(SequentialEventInfoAsset.Common.NewGame);
        yield return newGameEvent;
        if (newGameEvent.Status == AsyncOperationStatus.Succeeded)
        {
            _newGameEventInfo = newGameEvent.Result;
        }
        else
        {
            Debug.LogError("Failed to load NewGame event info.");
        }

        _isLoaded = true;   
    }

	private void OnContinueGameClick()
	{
        // Generate Load game event stream, but load most recent saved data
        GameEventManager.Instance.Submit(GameEventFactory.CreateFromInfo(_continueGameEventInfo));
	}

	private void OnNewGameClick()
	{
        // Generate new game event stream
        GameEventManager.Instance.Submit(GameEventFactory.CreateFromInfo(_newGameEventInfo));
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