using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;


public class SaveLoadUIBase : MonoBehaviour
{
    /****** Public Members ******/
    
    public void UpdateDataSlots()
    {
        _dataList = DataManager.Instance.GetAllUserData();
        SetDataSlots();
    }


    /****** Protected Members ******/

    protected SubUI SaveOrLoad
    {
        get { return _saveOrLoad; }
        set { _saveOrLoad = value; }
    }

    protected TextMeshProUGUI LabelText
    {
        get { return _labelText; }
        set { _labelText = value; }
    }

    protected TextMeshProUGUI ConfirmText
    {
        get { return _confirmText; }
        set { _confirmText = value; }
    }

    protected DataSlot SelectedSlot
    {
        get { return _selectedSlot; }
        private set { _selectedSlot = value; }
    }

    protected bool TryClosingConfirmPanel()
    {
        if (_confirmPanel.gameObject.activeInHierarchy)
        {
            _confirmPanel.gameObject.SetActive(false);
            return true;
        }
        
        return false;
    }

    protected void SetConfirmButtonAction(Action onConfirmAction)
    {
        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(() => onConfirmAction?.Invoke());
    }

    protected void ResetUISettings()
    {
        _selectedSlot           =   null;
        _currentPage            =   1;
        _saveOrLoad             =   SubUI.SubUICount;
        _pageNumberText.text    =   _currentPage + "/" + (DataManager.SlotNumber / _slots.childCount);
        
        _previousButton.gameObject.SetActive(false);
        _nextButton.gameObject.SetActive(true);
    }

    protected virtual void Start()
    {
        _currentPage    =   1;
        _saveOrLoad     =   SubUI.SubUICount;

        // Set Slot Info
        _dataList = DataManager.Instance.GetAllUserData();
        SetDataSlots();
    }


    /****** Private Members ******/

    private const string _LabelName         =   "SaveLoad Label";
    private const string _PageNumberName    =   "Page Number Text";
    private const string _PageButtonsName   =   "Page Buttons";
    private const string _SlotsName         =   "Slots";
    private const string _ConfirmPanelName  =   "Confirm Panel";
    private TextMeshProUGUI _labelText;
    private TextMeshProUGUI _pageNumberText;
    private TextMeshProUGUI _confirmText;
    private Transform _slots;
    private Transform _confirmPanel;
    private Button _previousButton;
    private Button _nextButton;
    private Button _confirmButton;
    private Button _cancelButton;
    private List<DataSlot> _slotList;
    private List<SaveData> _dataList;
    private DataSlot _selectedSlot;
    private int _currentPage;
    private SubUI _saveOrLoad;


    private void Awake()
    {
        // Load UI objects
        _labelText              =   transform.Find(_LabelName).GetChild(0).GetComponent<TextMeshProUGUI>();
        _previousButton         =   transform.Find(_PageButtonsName).GetChild(0).GetComponent<Button>();
        _nextButton             =   transform.Find(_PageButtonsName).GetChild(1).GetComponent<Button>();
        _slots                  =   transform.Find(_SlotsName);
        _pageNumberText         =   transform.Find(_PageNumberName).GetComponent<TextMeshProUGUI>();
        _pageNumberText.text    =   _currentPage + "/" + (DataManager.SlotNumber / _slots.childCount);
        _confirmPanel           =   transform.Find(_ConfirmPanelName);
        _confirmText            =   _confirmPanel.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        _confirmButton          =   _confirmPanel.GetChild(0).GetChild(1).GetComponent<Button>();
        _cancelButton           =   _confirmPanel.GetChild(0).GetChild(2).GetComponent<Button>();
        _slotList               =   new List<DataSlot>();

        // Add listeners
        _previousButton.onClick.AddListener(OnPreviousButtonClick);
        _nextButton.onClick.AddListener(OnNextButtonClick);
        _cancelButton.onClick.AddListener(OnCancelButtonClick);

        // Load data slots
        for (int i = 0; i < _slots.childCount; i++)
        {
            int index = i;
            _slotList.Add(new DataSlot(_slots.GetChild(i)));
            _slotList[i].slotButton.onClick.AddListener(() => OnSlotClick(index));
        }
    }

    // Load user data and set to data slots
    private void SetDataSlots()
    {
        int addNum = (_currentPage - 1) * _slots.childCount;

        for (int i = 0; i < _slots.childCount; i++)
        {
            _slotList[i].SetSlotInfo(_dataList[i + addNum], i + addNum);
        }
    }

    private void OnSlotClick(int index)
    {
        // Return when slot is empty and it's load UI
        if (_slotList[index].slotData == null && _saveOrLoad == SubUI.Load)
            return;

        _confirmPanel.gameObject.SetActive(true);
        _selectedSlot = _slotList[index];
    }

    // On next page button click
    private void OnNextButtonClick()
    {
        _currentPage++;

        // Is this Last page?
        if (_currentPage == DataManager.SlotNumber / _slots.childCount) _nextButton.gameObject.SetActive(false);
        else _previousButton.gameObject.SetActive(true);

        _pageNumberText.text = _currentPage + "/" + (DataManager.SlotNumber / _slots.childCount);

        // Update data slots
        SetDataSlots();
    }

    private void OnPreviousButtonClick()
    {
        _currentPage--;

        // Is this Last page?
        if (_currentPage == 1) _previousButton.gameObject.SetActive(false);
        else _nextButton.gameObject.SetActive(true);

        _pageNumberText.text = _currentPage + "/" + (DataManager.SlotNumber / _slots.childCount);

        SetDataSlots();
    }

    public void OnCancelButtonClick()
    {
        _confirmPanel.gameObject.SetActive(false);
    }
}