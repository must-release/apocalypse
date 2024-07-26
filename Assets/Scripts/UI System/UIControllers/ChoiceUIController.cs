using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIEnums;
using TMPro;

public class ChoiceUIController : MonoBehaviour, IUIContoller
{
    /****** Private fields ******/
    private string choicelUIName = "Choice UI";
    private string inputOptionName = "Input Option";
    private string submitButtonName = "Submit Button";
    private string inputFieldName = "Input Field";
    private Transform choiceUI;
    private List<ChoiceOption> choiceOptionList;
    private Transform inputOption;
    private Button inputOptionSubmitButton;
    private TMP_InputField inputField;

    /****** Single tone instance ******/
    public static ChoiceUIController Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Title UI object
            choiceUI = transform.Find(choicelUIName);
            if (choiceUI == null)
            {
                Debug.Log("Choice UI Initialization Error");
                return;
            }
            choiceOptionList = new List<ChoiceOption>();
            for (int i = 0; i < choiceUI.childCount - 1; i++)
            {
                int index = i;
                choiceOptionList.Add(new ChoiceOption(choiceUI.GetChild(i)));
                choiceOptionList[i].optionButton.onClick.AddListener(() => OnChoiceSelect(index));
            }
            inputOption = choiceUI.Find(inputOptionName);
            inputOptionSubmitButton = inputOption.Find(submitButtonName).GetComponent<Button>();
            inputOptionSubmitButton.onClick.AddListener(()=>OnChoiceSelect(choiceUI.childCount));
            inputField = inputOption.Find(inputFieldName).GetComponent<TMP_InputField>();
        }
    }

    /****** Methods ******/

    // Enter Choice UI state
    public void StartUI()
    {
        // Active Choice UI object
        choiceUI.gameObject.SetActive(true);

        // Set Choice UI
        SetChoiceUI();
    }

    // Update Choice UI
    public void UpdateUI()
    {
        
    }

    // Exit Choice UI state
    public void EndUI()
    {
        // Reset input field
        inputField.text = "";
        //inputField.caretPosition = 0;
        //inputField.selectionAnchorPosition = 0;
        //inputField.selectionFocusPosition = 0;

        // Turn off choice UI
        inputOption.gameObject.SetActive(false);
        choiceOptionList.ForEach((option) => option.SetOptionInactive());
        choiceUI.gameObject.SetActive(false);
        inputField.DeactivateInputField();
    }

    public void Cancel()
    {

    }

    // Set Choice UI
    public void SetChoiceUI()
    {
        // Get choice info
        List<string> choiceList = UIModel.Instance.ChoiceList;

        if(choiceList == null) // Enable input option
        {
            inputOption.gameObject.SetActive(true);
        }
        else // Enable choice options
        {
            for(int i = 0; i < choiceOptionList.Count; i++)
            {
                choiceOptionList[i].SetOption(choiceList[i]);
            }
        }
    }

    // Process selected choice option
    public void OnChoiceSelect(int index)
    {
        // Update selected choice
        if (index == choiceUI.childCount) // When player wrote text
        {
            UIModel.Instance.SelectedChoice = inputField.text;
        }
        else // When player selected choice option
        {
            UIModel.Instance.SelectedChoice = choiceOptionList[index].optionText.text;
        }

        // Turn Choice UI off
        UIController.Instance.TurnSubUIOff(SUBUI.CHOICE);
    }
}

