using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


/* Part of InputManager which manages SaveLoad UI logic */

public class SaveLoadUIState : MonoBehaviour, IUIState
{
    public static SaveLoadUIState Instance;

    private string saveLoadUIName = "SaveLoad UI";
    private string labelName = "SaveLoad Label";
    private string pageNumberName = "Page Number Text";
    private string pageButtonsName = "Page Buttons";
    private string slotsName = "Slots";
    private string confirmPanelName = "Confirm Panel";

    private UIManager.STATE myState;

    private Transform saveLoadUI;
    private TextMeshProUGUI labelText;
    private TextMeshProUGUI pageNumberText;
    private int currentPage = 1;
    private Button previousButton;
    private Button nextButton;
    private Transform slots;
    private Transform confirmPanel;
    private TextMeshProUGUI confirmText;
    private Button confirmButton;
    private Button cancelButton;
    private List<DataSlot> slotList;
    private List<UserData> dataList;
    private DataSlot selectedSlot;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Save Load UI object
            saveLoadUI = transform.Find(saveLoadUIName);
            if (saveLoadUI == null)
            {
                Debug.Log("Title UI Initialization Error");
                return;
            }

            // Load UI objects
            labelText = saveLoadUI.Find(labelName).GetChild(0).GetComponent<TextMeshProUGUI>();
            previousButton = saveLoadUI.Find(pageButtonsName).GetChild(0).GetComponent<Button>();
            previousButton.onClick.AddListener(OnPreviousButtonClick);
            nextButton = saveLoadUI.Find(pageButtonsName).GetChild(1).GetComponent<Button>();
            nextButton.onClick.AddListener(OnNextButtonClick);
            slots = saveLoadUI.Find(slotsName);
            pageNumberText = saveLoadUI.Find(pageNumberName).GetComponent<TextMeshProUGUI>();
            pageNumberText.text = currentPage + "/" + (DataManager.SLOT_NUM / slots.childCount);
            confirmPanel = saveLoadUI.Find(confirmPanelName);
            confirmText = confirmPanel.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            confirmButton = confirmPanel.GetChild(0).GetChild(1).GetComponent<Button>();
            cancelButton = confirmPanel.GetChild(0).GetChild(2).GetComponent<Button>();
            cancelButton.onClick.AddListener(Cancel);
            slotList = new List<DataSlot>();

            // Load data slots
            for (int i = 0; i < slots.childCount; i++)
            {
                slotList.Add(new DataSlot(slots.GetChild(i)));
            }
        }
    }

    // Enter SaveLoad UI state
    public void StartUI()
    {
        confirmButton.onClick.RemoveAllListeners();

        // Check if it is load or save
        if (UIManager.Instance.CurrentState == UIManager.STATE.SAVE)
        {
            confirmText.text = "Save Data?";
            labelText.text = "SAVE";
            confirmButton.onClick.AddListener(SaveAtSelectedSlot);
            myState = UIManager.STATE.SAVE;
        }
        else
        {
            confirmText.text = "Load Data?";
            labelText.text = "LOAD";
            confirmButton.onClick.AddListener(LoadSavedData);
            myState = UIManager.STATE.LOAD;
        }

        // Set Slot Info
        dataList = DataManager.Instance.LoadAllUserData();
        LoadDataSlots();

        // Active SaveLoad UI object
        saveLoadUI.gameObject.SetActive(true);
    }

    // Exit Title UI state
    public void EndUI()
    {
        // Inactive SaveLoad UI object
        saveLoadUI.gameObject.SetActive(false);

        // Reset UI objects & Info
        currentPage = 1;
        pageNumberText.text = currentPage + "/" + (DataManager.SLOT_NUM / slots.childCount);
    }

    // return to previous UI
    public void Cancel()
    {
        // check if confirm panel is on.
        if (confirmPanel.gameObject.activeInHierarchy)
        {
            confirmPanel.gameObject.SetActive(false);
            slotList[0].slotButton.Select();
        }
        else
            UIManager.Instance.ChangeToPreviousState();
    }

    // Load saved data of the selected slot
    private void LoadSavedData()
    {
        DataManager.Instance.SetGameLoadData(selectedSlot.slotData);
        selectedSlot = null;
        Instance.confirmPanel.gameObject.SetActive(false);
    }

    // Load user data to data slots
    private void LoadDataSlots()
    {
        int addNum = (currentPage - 1) * slots.childCount;

        for (int i = 0; i < slots.childCount; i++)
        {
            slotList[i].setSlotInfo(dataList[i + addNum], i + addNum);
        }

        // In case of console, select first button
        if (IUIState.isConsole)
        {
            if(UIManager.Instance.CurrentState == UIManager.STATE.SAVE && currentPage == 1)
            {
                slotList[1].slotButton.Select();
            }
            else
            {
                slotList[0].slotButton.Select();
            }
        }
    }

    // Save current Player Data at the selected slot
    private void SaveAtSelectedSlot()
    {
        // Save current player data
        DataManager.Instance.SaveUserData(selectedSlot.slotNumber);
        selectedSlot = null;
        Instance.confirmPanel.gameObject.SetActive(false);
        slotList[0].slotButton.Select();
    }

    // On next page button click
    private void OnNextButtonClick()
    {
        currentPage++;

        // Is this Last page?
        if (currentPage == DataManager.SLOT_NUM / slots.childCount) nextButton.gameObject.SetActive(false);
        else previousButton.gameObject.SetActive(true);

        pageNumberText.text = currentPage + "/" + (DataManager.SLOT_NUM / slots.childCount);

        // Update data slots
        LoadDataSlots();
    }

    private void OnPreviousButtonClick()
    {
        currentPage--;

        // Is this Last page?
        if (currentPage == 1) previousButton.gameObject.SetActive(false);
        else nextButton.gameObject.SetActive(true);

        pageNumberText.text = currentPage + "/" + (DataManager.SLOT_NUM / slots.childCount);

        // Updata data slots
        LoadDataSlots();
    }

    public UIManager.STATE GetState()
    {
        return myState;
    }

    public void UpdateUI() { return; }
    public void Move(float move) { return; }
    public void Attack() { return; }
    public void Submit() { return; }
    public void Stop() { return; }


    class DataSlot
    {
        private Image screenShot;
        private TextMeshProUGUI saveTimeText;
        private TextMeshProUGUI playTimeText;
        private TextMeshProUGUI slotText;


        public UserData slotData;
        public Button slotButton;
        public int slotNumber;

        public DataSlot(Transform slot)
        {
            screenShot = slot.Find("Screenshot Image").GetComponent<Image>();
            saveTimeText = slot.Find("Data Info Box").Find("Save Time Text").GetComponent<TextMeshProUGUI>();
            playTimeText = slot.Find("Data Info Box").Find("Play Time Text").GetComponent<TextMeshProUGUI>();
            slotText = slot.Find("Slot Text").GetComponent<TextMeshProUGUI>();
            slotButton = slot.GetComponent<Button>();
            slotButton.onClick.AddListener(OnSlotClick);
        }

        public void setSlotInfo(UserData data, int idx)
        {
            if (data!=null)
            {
                // When there is data
                slotData = data;
                if (data.ScreenShotImage != null)
                {
                    screenShot.sprite = Sprite.Create(data.ScreenShotImage,
                        new Rect(0.0f, 0.0f, data.ScreenShotImage.width, data.ScreenShotImage.height), new Vector2(0.5f, 0.5f), 100.0f);

                }
                saveTimeText.text = "Save Time: " + data.SaveTime;
                playTimeText.text = "Play TIme: " + data.PlayTime;
            }
            else
            {
                // When data is null
                slotData = null;
                screenShot.sprite = null;
                saveTimeText.text = "Save Time: - ";
                playTimeText.text = "Play TIme: - ";
            }

            slotNumber = idx;
            if (idx == 0)
            {
                slotText.text = "AUTO";

                // Can not save at auto save slot
                if(UIManager.Instance.CurrentState == UIManager.STATE.SAVE)
                {
                    slotButton.interactable = false;
                }
                else
                {
                    slotButton.interactable = true;
                }
            }
            else
            {
                slotButton.interactable = true;
                slotText.text = "SLOT " + idx;
            }
        }

        private void OnSlotClick()
        {
            if (slotData == null && UIManager.Instance.CurrentState == UIManager.STATE.LOAD)
                return;
            Instance.confirmPanel.gameObject.SetActive(true);
            Instance.selectedSlot = this;

            // In case of console, select button
            if (IUIState.isConsole)
                Instance.confirmButton.Select();
        }
    }
}