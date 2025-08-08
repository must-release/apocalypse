using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        _curUIView?.ExitUI();

        UIModel.Instance.CurrentBaseUI = baseUI;
        SetUIView(baseUI);

        _curUIView.EnterUI();
    }

    public void TurnSubUIOn(SubUI subUI)
    {
        // Stack sub UI
        UIModel.Instance.PushNewSubUI(subUI);

        // Set current UI Controller to sub UI
        SetUIView(subUI);

        // Start Sub UI
        _curUIView.EnterUI();
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
        _curUIView.ExitUI();
        UIModel.Instance.PopCurrentSubUI();

        // Check what UI comes next
        if (UIModel.Instance.CurrentSubUI == SubUI.None) // When there is no sub UI left in the stack
        {
            // Set curUIController to Base UI
            SetUIView(UIModel.Instance.CurrentBaseUI);
        }
        else // If there is sub UI left in the stack
        {
            // Set previous sub UI to current UI controller
            SetUIView(UIModel.Instance.CurrentSubUI);

            // Update Sub UI
            _curUIView.UpdateUI();
        }
    }

    public void TurnEverySubUIOff()
    {
        while (UIModel.Instance.CurrentSubUI != SubUI.None)
        {
            _curUIView.ExitUI();
            UIModel.Instance.PopCurrentSubUI();

            SetUIView(UIModel.Instance.CurrentSubUI);
        }
    }

    public void GetCurrentUI(out BaseUI baseUI, out SubUI subUI)
    {
        baseUI = UIModel.Instance.CurrentBaseUI;
        subUI = UIModel.Instance.CurrentSubUI;
    }

    public IUIView<BaseUI> GetUIView(BaseUI baseUI)
    {
        Debug.Assert(_baseUIDictionary.ContainsKey(baseUI), $"{baseUI} does not exist in the base UI dictionary.");
        return _baseUIDictionary[baseUI];
    }

    public IUIView<SubUI> GetUIView(SubUI subUI)
    {
        Debug.Assert(_subUIDictionary.ContainsKey(subUI), $"{subUI} does not exist in the sub UI dictionary.");
        return _subUIDictionary[subUI];
    }
    
    public void SetChoiceInfo(List<string> choiceList)
    {
        UIModel.Instance.ChoiceList = choiceList;
        UIModel.Instance.SelectedChoice = null;
    }

    public string GetSelectedChoice() { return UIModel.Instance.SelectedChoice; }
    public void CancelCurrentUI() { _curUIView.Cancel(); }


    /***** Private Members ******/

    [SerializeField] private Transform _baseUITransform;
    [SerializeField] private Transform _subUITransform;

    private Dictionary<BaseUI, IUIView<BaseUI>> _baseUIDictionary   = new();
    private Dictionary<SubUI, IUIView<SubUI>>   _subUIDictionary    = new();

    private IUIView _curUIView;
    private bool    _isStoryPanelClicked;
    private bool    _isLoaded;

    private void Awake()
    {
        Debug.Assert(null != _baseUITransform, "Base UI Transform is not assigned in the editor.");
        Debug.Assert(null != _subUITransform, "Sub UI Transform is not assigned in the editor.");

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

            _baseUIDictionary.Add(baseUIEntry.BaseUIType, uiObject.GetComponent<IUIView<BaseUI>>());

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

            _subUIDictionary.Add(subUIEntry.SubUIType, uiObject.GetComponent<IUIView<SubUI>>());

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

    private void SetUIView(BaseUI baseUI)
    {
        if (_baseUIDictionary.TryGetValue(baseUI, out IUIView<BaseUI> controller))
        {
            _curUIView = controller;
        }
        else
        {
            Debug.Log("Such base UI does not exists: " + baseUI.ToString());
        }
    }

    private void SetUIView(SubUI subUI)
    {
        if(subUI == SubUI.None)
        {
            SetUIView(UIModel.Instance.CurrentBaseUI);
        }
        else if (_subUIDictionary.TryGetValue(subUI, out IUIView<SubUI> controller))
        {
            _curUIView = controller;
        }
        else
        {
            Debug.Log("Such sub UI does not exists: " + subUI.ToString());
        }
    }
}

public interface IUIView
{
    public void EnterUI();
    public void UpdateUI();
    public void ExitUI();
    public void Cancel();
}

public interface IUIView<TEnum> : IUIView
    where TEnum : Enum
{
    TEnum UIType { get; }
}