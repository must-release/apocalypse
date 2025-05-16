using UnityEngine;
using UnityEngine.UI;
using UIEnums;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AddressableAssets;
using AssetEnums;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameOverUIController : MonoBehaviour, IUIController<BaseUI>, IAsyncLoadObject
{
    /****** Public Members ******/

    public bool IsLoaded => _isLoaded;

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

    public BaseUI UIType => BaseUI.GameOver;


    /****** Private Members ******/

    private const string    _ContinueButtonName         = "Continue Button";
    private const string    _TitleButtonName            = "Title Button";
    private const string    _ButtonsName                = "Buttons";
    private GameEventInfo   _continueGameEventInfo      = null;
    private GameEventInfo   _returnToTitleEventInfo     = null;
    private Button          _continueBtn                = null;
    private Button          _titleBtn                   = null;
    private bool            _isLoaded                   = false;

    private void Awake()
    {
        Transform buttonsTransform = transform.Find(_ButtonsName);
        if ( null == buttonsTransform )
        {
            Debug.LogError("Can not Find " + _ButtonsName);
            return;
        }

        _continueBtn    =   buttonsTransform.Find(_ContinueButtonName).GetComponent<Button>();
        _titleBtn       =   buttonsTransform.Find(_TitleButtonName).GetComponent<Button>();

        _continueBtn.onClick.AddListener(OnContinueClick);
        _titleBtn.onClick.AddListener(ReturnToTitle);
    }

    private void Start()
    {
        StartCoroutine(LoadGameEventInfos());
    }

    private IEnumerator LoadGameEventInfos()
    {
        var continueGameEventInfo = Addressables.LoadAssetAsync<GameEventInfo>(SequentialEventInfoAsset.Common.ContinueGame);
        yield return continueGameEventInfo;
        if ( continueGameEventInfo.Status == AsyncOperationStatus.Succeeded )
        {
            _continueGameEventInfo = continueGameEventInfo.Result;
        }
        else
        {
            Debug.LogError("Load Failed : [" + SequentialEventInfoAsset.Common.ContinueGame + "]");
            yield break;
        }

        var returnToTitleEventInfo = Addressables.LoadAssetAsync<GameEventInfo>(SequentialEventInfoAsset.Common.ReturnToTitle);
        yield return returnToTitleEventInfo;
        if ( returnToTitleEventInfo.Status == AsyncOperationStatus.Succeeded )
        {
            _returnToTitleEventInfo = returnToTitleEventInfo.Result;
        }
        else
        {
            Debug.LogError("Load Failed : [" + SequentialEventInfoAsset.Common.ReturnToTitle + "]");
            yield break;
        }

        _isLoaded = true;
    }

    private void OnContinueClick()
    {
        GameEventManager.Instance.Submit(GameEventFactory.CreateFromInfo(_continueGameEventInfo));
    }

    private void ReturnToTitle()
    {
        UIController.Instance.TurnEverySubUIOff();

        GameEventManager.Instance.Submit(GameEventFactory.CreateFromInfo(_returnToTitleEventInfo));
    }
}

