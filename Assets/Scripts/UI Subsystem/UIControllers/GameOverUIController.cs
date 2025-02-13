using UnityEngine;
using UnityEngine.UI;
using UIEnums;
using System.Collections.Generic;

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

    public BaseUI GetUIType() { return BaseUI.GameOver; }


    /****** Private Members ******/

    private const string _ContinueButtonName = "Continue Button";
    private const string _TitleButtonName = "Title Button";
    private const string _ButtonsName = "Buttons";
    private Transform _confirmBox;
    private Button _continueBtn;
    private Button _titleBtn;

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

    }

    private void OnContinueClick()
    {
        GameEventProducer.Instance.GenerateLoadGameEventStream();
    }

    private void ReturnToTitle()
    {
        UIController.Instance.TurnEverySubUIOff();
        GameEventProducer.Instance.GenerateChangeSceneEventStream(SceneEnums.SCENE.TITLE);
    }
}

