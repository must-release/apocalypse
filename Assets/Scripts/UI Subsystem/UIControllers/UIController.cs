using UnityEngine;
using System;
using System.Collections.Generic;
using UIEnums;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using AssetEnums;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour, IAsyncLoadObject
{
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
    
    public bool IsLoaded() { return _isLoaded; }

    // Change Base UI.
    public void ChangeBaseUI(BaseUI baseUI)
    {
        _curUIController?.ExitUI();

        UIModel.Instance.CurrentBaseUI = baseUI;
        SetUIController(baseUI);

        _curUIController.EnterUI();
    }

    // Turn Sub UI On
    public void TurnSubUIOn(SubUI subUI)
    {
        // Stack sub UI
        UIModel.Instance.PushNewSubUI(subUI);

        // Set current UI Controller to sub UI
        SetUIController(subUI);

        // Start Sub UI
        _curUIController.EnterUI();
    }

    // Turn Sub UI Off
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

    // Turn Every Sub UI Off
    public void TurnEverySubUIOff()
    {
        while(UIModel.Instance.CurrentSubUI != SubUI.None)
        {
            // End current sub UI
            _curUIController.ExitUI();
            UIModel.Instance.PopCurrentSubUI();

            // Set current UI controller to previous sub UI
            SetUIController(UIModel.Instance.CurrentSubUI);
        }
    }

    // Get current UI. Return both base UI and sub UI
    public void GetCurrentUI(out BaseUI baseUI, out SubUI subUI)
    {
        baseUI = UIModel.Instance.CurrentBaseUI;
        subUI = UIModel.Instance.CurrentSubUI;
    }

    // Set choice info
    public void SetChoiceInfo(List<string> choiceList)
    {
        UIModel.Instance.ChoiceList = choiceList;
        UIModel.Instance.SelectedChoice = null;
    }

    // Get selected choice
    public string GetSelectedChoice() { return UIModel.Instance.SelectedChoice; }

    // Cancel current UI
    public void CancelCurrentUI() { _curUIController.Cancel(); }


    /***** Private Members ******/

    private const string _BaseUIName = "Base UI";
    private const string _SubUIName = "Sub UI";

    private IUIController _curUIController;
    private Dictionary<BaseUI, IUIController> _baseUIDictionary;
    private Dictionary<SubUI, IUIController> _subUIDictionary;
    private bool _isStoryPanelClicked;
    private bool _isLoaded;

    private void Awake()
    {
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
        _isStoryPanelClicked = false;
        _isLoaded = false;

        StartCoroutine(LoadUIControllers());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 감지
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0)
            {
                Debug.Log("클릭된 UI 오브젝트: " + results[0].gameObject.name);
            }
        }
    }

    // Load baseUI, subUI Controllers
    private IEnumerator LoadUIControllers()
    {
        _baseUIDictionary = new Dictionary<BaseUI, IUIController>();
        _subUIDictionary = new Dictionary<SubUI, IUIController>();

        Transform baseUITransform = transform.Find(_BaseUIName);
        Transform subUITransform = transform.Find(_SubUIName);

        foreach ( UIAsset.BaseUIName baseUI in Enum.GetValues(typeof(UIAsset.BaseUIName)))
        {
            string assetPath = UIAsset.PathPrefix + baseUI.ToString();
            AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(assetPath);
            yield return loadHandle;

            GameObject uiObject;
            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                uiObject = Instantiate(loadHandle.Result, baseUITransform);
                Debug.Log("Load Complete : [" + assetPath + "]");
            }
            else
            {
                Debug.LogError("Load Failed : [" + assetPath + "]");
                yield break;
            }

            foreach ( IUIController<BaseUI> uiController in uiObject.GetComponents<IUIController<BaseUI>>() )
            {
                BaseUI uiType = uiController.GetUIType();
                _baseUIDictionary.Add(uiType, uiController);
            }

            // Execute Awake, Start method of uiObject
            yield return null;

            uiObject.SetActive(false);
        }

        foreach ( UIAsset.SubUIName subUI in Enum.GetValues(typeof(UIAsset.SubUIName)))
        {
            string assetPath = UIAsset.PathPrefix + subUI.ToString();
            AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(assetPath);
            yield return loadHandle;

            GameObject uiObject;
            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                uiObject = Instantiate(loadHandle.Result, subUITransform);
                Debug.Log("Load Complete : [" + assetPath + "]");
            }
            else
            {
                Debug.LogError("Load Failed : [" + assetPath + "]");
                yield break;
            }

            foreach ( IUIController<SubUI> uiController in uiObject.GetComponents<IUIController<SubUI>>() )
            {
                SubUI uiType = uiController.GetUIType();
                _subUIDictionary.Add(uiType, uiController);
            }

            // Execute Awake, Start method of uiObject
            yield return null;

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

public interface IUIController<T> : IUIController
{
    public T GetUIType();
}