﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UIEnums;
using UnityEngine.AddressableAssets;
using AssetEnums;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class PauseUIController : MonoBehaviour, IUIController<SubUI>, IAsyncLoadObject
{
    /****** Public Members ******/

    public void EnterUI()
    {
        Time.timeScale = 0f;

        gameObject.SetActive(true);
    }

    public void UpdateUI()
    {

    }

    public void ExitUI()
    {
        EventSystem.current.SetSelectedGameObject(null);
        _confirmBox.gameObject.SetActive(false);
        gameObject.SetActive(false);

        Time.timeScale = 1f;
    }

    // Cancel Pause UI. Return to previous UI
    public void Cancel()
    {
        if(_confirmBox.gameObject.activeInHierarchy)
        {
            _confirmBox.gameObject.SetActive(false);
        }
        else
        {
            UIController.Instance.TurnSubUIOff(SubUI.Pause);
        }
    }

    public SubUI UIType  => SubUI.Pause;

    public bool IsLoaded => _isLoaded;
    

    /****** Private Members ******/
    
    private const string    _ConfirmBoxName             = "Confirm Box";
    private const string    _ConfirmButtonName          = "Confirm Button";
    private const string    _CancelButtonName           = "Cancel Button";
    private const string    _ButtonsName                = "Buttons";
    private GameEventInfo   _returnToTitleEventInfo     = null;
    private Transform       _confirmBox                 = null;
    private List<Button>    _buttonList                 = null;
    private bool            _isLoaded                   = false;

    private void Awake()
    {
        _confirmBox = transform.Find(_ConfirmBoxName);
        if ( null == _confirmBox )
        {
            Debug.LogError("Can not Find " + _ConfirmBoxName);
            return;
        }

        _buttonList = new List<Button>();
        Transform buttons = transform.Find(_ButtonsName);
        for (int i = 0; i < buttons.childCount; i++)
        {
            _buttonList.Add(buttons.GetChild(i).GetComponent<Button>());
        }

        // Add event listener to buttons
        _buttonList[0].onClick.AddListener(Cancel);
        _buttonList[1].onClick.AddListener(OnSaveButtonClick);
        _buttonList[2].onClick.AddListener(OnLoadButtonClick);
        _buttonList[3].onClick.AddListener(OnPreferenceButtonClick);
        _buttonList[4].onClick.AddListener(OnTitleButtonClick);
        _confirmBox.Find(_ConfirmButtonName).GetComponent<Button>().onClick.AddListener(ReturnToTitle);
        _confirmBox.Find(_CancelButtonName).GetComponent<Button>().onClick.AddListener(Cancel);
    }

    private void Start()
    {
        StartCoroutine(LoadGameEventInfos());
    }

    private IEnumerator LoadGameEventInfos()
    {
        var returnToTitleEvent =Addressables.LoadAssetAsync<GameEventInfo>(SequentialEventInfoAsset.Common.ReturnToTitle);
        yield return returnToTitleEvent;
        if (returnToTitleEvent.Status == AsyncOperationStatus.Succeeded)
        {
            _returnToTitleEventInfo = returnToTitleEvent.Result;
        }
        else
        {
            Debug.LogError("Can not load " + SequentialEventInfoAsset.Common.ReturnToTitle);
        }

        _isLoaded = true;
    }

    private void OnSaveButtonClick()
    {
        UIController.Instance.TurnSubUIOn(SubUI.Save);
    }

    private void OnLoadButtonClick()
    {
        UIController.Instance.TurnSubUIOn(SubUI.Load);
    }

    private void OnPreferenceButtonClick()
    {
        UIController.Instance.TurnSubUIOn(SubUI.Preference);
    }

    private void OnTitleButtonClick()
    {
        EventSystem.current.SetSelectedGameObject(null);
        _confirmBox.gameObject.SetActive(true);
    }
   
    private void ReturnToTitle()
    {
        UIController.Instance.TurnEverySubUIOff();

        GameEventManager.Instance.Submit(GameEventFactory.CreateFromInfo(_returnToTitleEventInfo));
    }
}
