using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using AssetEnums;
using UnityEngine.EventSystems;
using NUnit.Framework;

public class UIController : MonoBehaviour, IAsyncLoadObject
{
    /****** Public Members ******/

    public static UIController Instance { get; private set; }

    public bool IsStoryPanelClicked
    {
        get 
        {
            bool _clicked = _isStoryPanelClicked;
            _isStoryPanelClicked = false;
            return _clicked; 
        }
        set { _isStoryPanelClicked = value; }
    }
    public bool IsLoaded => _isLoaded;

    public void ChangeBaseUI(BaseUI baseUI)
    {
        _curUIController?.ExitUI();

        UIModel.Instance.CurrentBaseUI = baseUI;
        SetUIController(baseUI);

        _curUIController.EnterUI();
    }

    public void TurnSubUIOn(SubUI subUI)
    {
        // Stack sub UI
        UIModel.Instance.PushNewSubUI(subUI);

        // Set current UI Controller to sub UI
        SetUIController(subUI);

        // Start Sub UI
        _curUIController.EnterUI();
    }

    public void TurnSubUIOff(SubUI subUI)
    {
        // Check if it is a right call
        if (UIModel.Instance.CurrentSubUI != subUI || subUI == SubUI.None)
        {
            Debug.LogError("Sub UI Mismatch \n" + subUI + '\n' + UIModel.Instance.CurrentSubUI);
            return;
        }
                    
        // End current Sub UI
        _curUIController.ExitUI();
        UIModel.Instance.PopCurrentSubUI();

        // Check what UI comes next
        if(UIModel.Instance.CurrentSubUI == SubUI.None) // When there is no sub UI left in the stack
        {
            // Set curUIController to Base UI
            SetUIController(UIModel.Instance.CurrentBaseUI);
        }
        else // If there is sub UI left in the stack
        {
            // Set previous sub UI to current UI controller
            SetUIController(UIModel.Instance.CurrentSubUI);

            // Update Sub UI
            _curUIController.UpdateUI();
        }
    }

    public void TurnEverySubUIOff()
    {
        while(UIModel.Instance.CurrentSubUI != SubUI.None)
        {
            _curUIController.ExitUI();
            UIModel.Instance.PopCurrentSubUI();

            SetUIController(UIModel.Instance.CurrentSubUI);
        }
    }

    public void GetCurrentUI(out BaseUI baseUI, out SubUI subUI)
    {
        baseUI = UIModel.Instance.CurrentBaseUI;
        subUI = UIModel.Instance.CurrentSubUI;
    }

    public void SetChoiceInfo(List<string> choiceList)
    {
        UIModel.Instance.ChoiceList = choiceList;
        UIModel.Instance.SelectedChoice = null;
    }

    public string GetSelectedChoice() { return UIModel.Instance.SelectedChoice; }
    public void CancelCurrentUI() { _curUIController.Cancel(); }


    /***** Private Members ******/

    [SerializeField] private Transform _baseUITransform = null;
    [SerializeField] private Transform _subUITransform  = null;

    private Dictionary<BaseUI, IUIController>   _baseUIDictionary   = new();
    private Dictionary<SubUI, IUIController>    _subUIDictionary    = new();

    private IUIController   _curUIController        = null;
    private bool            _isStoryPanelClicked    = false;
    private bool            _isLoaded               = false;

    private void Awake()
    {
        Assert.IsTrue(null != _baseUITransform, "Base UI Transform is not assigned in the editor.");
        Assert.IsTrue(null != _subUITransform, "Sub UI Transform is not assigned in the editor.");

        if (null == Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartCoroutine(LoadUIControllers());
    }

    private void Update()
    {
    }

    private IEnumerator LoadUIControllers()
    {
        var handle = Addressables.LoadAssetAsync<UIAsset>(AssetPath.UIAsset);
        yield return handle;

        if (AsyncOperationStatus.Failed == handle.Status)
        {
            Debug.LogError("Failed to load UIAsset");
            yield break;
        }

        var baseUIAsset = handle.Result.BaseUIAssets;
        var subUIAsset  = handle.Result.SubUIAssets;

        foreach(var baseUIEntry in baseUIAsset)
        {
            GameObject uiObject = Instantiate(baseUIEntry.BaseUIPrefab, _baseUITransform);
            uiObject.name = baseUIEntry.BaseUIType.ToString() + " UI";

            _baseUIDictionary.Add(baseUIEntry.BaseUIType, uiObject.GetComponent<IUIController<BaseUI>>());

            // Execute Awake, Start method of uiObject
            yield return null;

            if (uiObject.TryGetComponent(out IAsyncLoadObject asyncObject))
            {
                yield return new WaitUntil(() => asyncObject.IsLoaded);
            }

            uiObject.SetActive(false);
        }

        foreach (var subUIEntry in subUIAsset)
        {
            GameObject uiObject = Instantiate(subUIEntry.SubUIPrefab, _subUITransform);
            uiObject.name = subUIEntry.SubUIType.ToString() + " UI";

            _subUIDictionary.Add(subUIEntry.SubUIType, uiObject.GetComponent<IUIController<SubUI>>());

            // Execute Awake, Start method of uiObject
            yield return null;

            if (uiObject.TryGetComponent(out IAsyncLoadObject asyncObject))
            {
                yield return new WaitUntil(() => asyncObject.IsLoaded);
            }

            uiObject.SetActive(false);
        }
    
        _isLoaded = true;
    }

    private void SetUIController(BaseUI baseUI)
    {
        if (_baseUIDictionary.TryGetValue(baseUI, out IUIController controller))
        {
            _curUIController = controller;
        }
        else
        {
            Debug.Log("Such base UI does not exists: " + baseUI.ToString());
        }
    }

    private void SetUIController(SubUI subUI)
    {
        if(subUI == SubUI.None)
        {
            SetUIController(UIModel.Instance.CurrentBaseUI);
        }
        else if (_subUIDictionary.TryGetValue(subUI, out IUIController controller))
        {
            _curUIController = controller;
        }
        else
        {
            Debug.Log("Such sub UI does not exists: " + subUI.ToString());
        }
    }
}

public interface IUIController
{
    public void EnterUI();
    public void UpdateUI();
    public void ExitUI();
    public void Cancel();
}

public interface IUIController<TEnum> : IUIController
    where TEnum : Enum
{
    TEnum UIType { get; }
}