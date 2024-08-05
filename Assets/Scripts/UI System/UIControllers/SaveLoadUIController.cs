using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UIEnums;


/* Part of UIContoller which manages SaveLoad SubUI logic */

public class SaveLoadUIController : MonoBehaviour, IUIContoller
{
    /****** Private fields ******/
    private string saveLoadUIName = "SaveLoad UI";
    private string labelName = "SaveLoad Label";
    private string pageNumberName = "Page Number Text";
    private string pageButtonsName = "Page Buttons";
    private string slotsName = "Slots";
    private string confirmPanelName = "Confirm Panel";

    private Transform saveLoadUI;
    private TextMeshProUGUI labelText;
    private TextMeshProUGUI pageNumberText;
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

    private int currentPage = 1;
    private SUBUI saveOrLoad = SUBUI.NONE;


    /****** Single tone instance ******/
    public static SaveLoadUIController Instance;


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
                int index = i;
                slotList.Add(new DataSlot(slots.GetChild(i)));
                slotList[i].slotButton.onClick.AddListener(() => OnSlotClick(index));
            }
        }
    }

    // Initialize SaveLoad UI
    public void Start()
    {
        // Set Slot Info
        dataList = DataManager.Instance.GetAllUserData();
        SetDataSlots();
    }

    /****** Methods ******/

    // Enter SaveLoad UI
    public void StartUI()
    {
        // Get current UI state and set UI objects
        saveOrLoad = UIModel.Instance.CurrentSubUI;
        SetSaveLoadUI();

        // Active SaveLoad UI object
        saveLoadUI.gameObject.SetActive(true);
    }


    // Update SaveLoad UI
    public void UpdateUI()
    {
        // Set Slot Info
        dataList = DataManager.Instance.GetAllUserData();
        SetDataSlots();
    }

    // Exit SaveLoad UI
    public void EndUI()
    {
        // Inactive SaveLoad UI object
        saveLoadUI.gameObject.SetActive(false);

        // Reset UI objects & Info
        ResetUISettings();
    }

    // Cancel SaveLoad UI. Return to previous UI
    public void Cancel()
    {
        if (confirmPanel.gameObject.activeInHierarchy) // check if confirm panel is on.
        {
            confirmPanel.gameObject.SetActive(false);
        }
        else // Turn save load UI off
        {
            UIController.Instance.TurnSubUIOff(saveOrLoad);
        }
    }

    // Set SaveLoad UI objects
    private void SetSaveLoadUI()
    {
        if (saveOrLoad == SUBUI.SAVE)
        {
            confirmText.text = "Save Data?";
            labelText.text = "SAVE";
            confirmButton.onClick.AddListener(SaveAtSelectedSlot);
        }
        else if (saveOrLoad == SUBUI.LOAD)
        {
            confirmText.text = "Load Data?";
            labelText.text = "LOAD";
            confirmButton.onClick.AddListener(LoadSavedData);
        }
        else
        {
            Debug.Log("SubUI error: must be save or load");
        }
    }

    // Load user data and set to data slots
    private void SetDataSlots()
    {
        int addNum = (currentPage - 1) * slots.childCount;

        for (int i = 0; i < slots.childCount; i++)
        {
            slotList[i].SetSlotInfo(dataList[i + addNum], i + addNum);
        }
    }

    // Load saved data of the selected slot
    private void LoadSavedData()
    {
        // Get slot number
        int slotNum = selectedSlot.slotNumber;

        // Close confirm panel
        confirmPanel.gameObject.SetActive(false);

        // Turn every sub UI off
        UIController.Instance.TurnEverySubUIOff();

        // Generate Load Game Event Stream. Load data of the selected slot
        GameEventProducer.Instance.GenerateLoadGameEventStream(slotNum);
    }

    // Save current player data at the selected slot
    private void SaveAtSelectedSlot()
    {
        // Close confirm panel
        Instance.confirmPanel.gameObject.SetActive(false);

        // Generate Save Game Event Stream. Save data at the selected slot
        GameEventProducer.Instance.GenerateSaveGameEventStream(selectedSlot.slotNumber);
    }

    // When player clicked data slot
    private void OnSlotClick(int index)
    {
        // Return when slot is empty and it's load UI
        if (slotList[index].slotData == null && saveOrLoad == SUBUI.LOAD)
            return;

        confirmPanel.gameObject.SetActive(true);
        selectedSlot = slotList[index];
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
        SetDataSlots();
    }

    private void OnPreviousButtonClick()
    {
        currentPage--;

        // Is this Last page?
        if (currentPage == 1) previousButton.gameObject.SetActive(false);
        else nextButton.gameObject.SetActive(true);

        pageNumberText.text = currentPage + "/" + (DataManager.SLOT_NUM / slots.childCount);

        // Updata data slots
        SetDataSlots();
    }

    // Reset UI objects & Info
    private void ResetUISettings()
    {
        selectedSlot = null;
        currentPage = 1;
        saveOrLoad = SUBUI.NONE;
        pageNumberText.text = currentPage + "/" + (DataManager.SLOT_NUM / slots.childCount);
        previousButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);
        confirmButton.onClick.RemoveAllListeners();
    }
}