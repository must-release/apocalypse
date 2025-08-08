using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ChoiceUIController : MonoBehaviour, IUIView<SubUI>
{
    /****** Public Members ******/

    public void EnterUI()
    {
        gameObject.SetActive(true);
        SetChoiceUI();
    }

    public void UpdateUI()
    {
        
    }

    public void ExitUI()
    {
        // Reset input field
        _inputField.text = "";

        // Turn off choice UI
        _inputOption.gameObject.SetActive(false);
        _choiceOptionList.ForEach((option) => option.SetOptionInactive());
        gameObject.SetActive(false);
        _inputField.DeactivateInputField();
    }

    public void Cancel()
    {

    }

    public SubUI UIType => SubUI.Choice;


    /****** Private Members ******/

    private const string _InputOptionName = "Input Option";
    private const string _SubmitButtonName = "Submit Button";
    private const string _InputFieldName = "Input Field";
    private List<ChoiceOption> _choiceOptionList;
    private Transform _inputOption;
    private Button _inputOptionSubmitButton;
    private TMP_InputField _inputField;

    private void Awake()
    {
        _choiceOptionList = new List<ChoiceOption>();
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            int index = i;
            _choiceOptionList.Add(new ChoiceOption(transform.GetChild(i)));
            _choiceOptionList[i].optionButton.onClick.AddListener(() => OnChoiceSelect(index));
        }
        _inputOption = transform.Find(_InputOptionName);
        _inputOptionSubmitButton = _inputOption.Find(_SubmitButtonName).GetComponent<Button>();
        _inputOptionSubmitButton.onClick.AddListener(()=>OnChoiceSelect(transform.childCount));
        _inputField = _inputOption.Find(_InputFieldName).GetComponent<TMP_InputField>();
    }

    private void Start() 
    { 
        
    }

    private void SetChoiceUI()
    {
        // Get choice info
        List<string> choiceList = UIModel.Instance.ChoiceList;

        if(null == choiceList) // Enable input option
        {
            _inputOption.gameObject.SetActive(true);
        }
        else // Enable choice options
        {
            for(int i = 0; i < _choiceOptionList.Count; i++)
            {
                _choiceOptionList[i].SetOption(choiceList[i]);
            }
        }
    }

    // Process selected choice option
    public void OnChoiceSelect(int index)
    {
        // Update selected choice
        if ( transform.childCount == index ) // When player wrote text
        {
            UIModel.Instance.SelectedChoice = _inputField.text;
        }
        else // When player selected choice option
        {
            UIModel.Instance.SelectedChoice = _choiceOptionList[index].optionText.text;
        }

        UIController.Instance.TurnSubUIOff(SubUI.Choice);
    }
}

