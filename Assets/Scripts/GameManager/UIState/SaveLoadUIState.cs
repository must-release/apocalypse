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
    private string slotsName = "slots";

    private Transform saveLoadUI;
    private TextMeshProUGUI labelText;
    private TextMeshProUGUI pageNumberText;
    private Button previousButton;
    private Button nextButton;
    private Transform slots;
    private List<DataSlot> slotList;
    


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Save Load UI object
            saveLoadUI = FindObjectOfType<Canvas>().transform.Find(saveLoadUIName);
            if (saveLoadUI == null)
            {
                Debug.Log("Title UI Initialization Error");
                return;
            }

            // Load UI objects
            labelText = saveLoadUI.Find(labelName).GetChild(0).GetComponent<TextMeshProUGUI>();
            pageNumberText = saveLoadUI.Find(pageNumberName).GetComponent<TextMeshProUGUI>();
            previousButton = saveLoadUI.Find(pageButtonsName).GetChild(0).GetComponent<Button>();
            nextButton = saveLoadUI.Find(pageButtonsName).GetChild(1).GetComponent<Button>();
            slots = saveLoadUI.Find(slotsName);

            // Load data slots
            for(int i=0; i<slots.childCount; i++)
            {

            }
        }
    }


    // Enter SaveLoad UI state
    public void StartUI()
    {
        //Check it it is load or save
        if (InputManager.Instance.SaveOrLoad == InputManager.STATE.SAVE)
            labelText.text = "SAVE";
        else
            labelText.text = "LOAD";

        // Active Title UI object
        saveLoadUI.gameObject.SetActive(true);
    }

    // Exit Title UI state
    public void EndUI()
    {
        // Inactive Title UI object
        saveLoadUI.gameObject.SetActive(false);
    }



    // return to previous UI
    public void Cancel()
    {
        InputManager.Instance.ChangeState(InputManager.STATE.PREVIOUS, true);
    }

    public void UpdateUI() { return; }
    public void Move(float move) { return; }
    public void Attack() { return; }
    public void Submit() { return; }
    public void Stop() { return; }


    class DataSlot
    {
        Transform slotObject;
        Image screenShot;
        TextMeshProUGUI saveTimeText;
        TextMeshProUGUI playTimeText;
        TextMeshProUGUI slotText;

        DataSlot(Transform slot)
        {
            slotObject = slot;
            screenShot = slot.Find("Screenshot Image").GetComponent<Image>();
            saveTimeText = slot.Find("Data Info Box").Find("Save Time Text").GetComponent<TextMeshProUGUI>();
            playTimeText = slot.Find("Data Info Box").Find("Play Time Text").GetComponent<TextMeshProUGUI>();
            slotText = slot.Find("Slot Text").GetComponent<TextMeshProUGUI>();
        }
    }
}