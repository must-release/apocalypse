using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameOverUIController : MonoBehaviour, IUIController<BaseUI>
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

    public BaseUI UIType => BaseUI.GameOver;


    /****** Private Members ******/

    private const string    _ContinueButtonName         = "Continue Button";
    private const string    _TitleButtonName            = "Title Button";
    private const string    _ButtonsName                = "Buttons";
    private Button          _continueBtn;
    private Button          _titleBtn;

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

    private void OnContinueClick()
    {
        GameEventManager.Instance.Submit(GameEventFactory.CreateCommonEvent(CommonEventType.Continue));
    }

    private void ReturnToTitle()
    {
        UIController.Instance.TurnEverySubUIOff();

        GameEventManager.Instance.Submit(GameEventFactory.CreateCommonEvent(CommonEventType.ReturnToTitle));
    }
}

