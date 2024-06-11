using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/* Part of InputManager which manages Story UI logic */

public class StoryUIController : MonoBehaviour, IUIContoller
{

    /****** Private fields ******/
    private string storyUIName = "Story UI";
    private string choicePanelName = "Choice Panel";
    private string inputOptionName = "Input Option";
    private string submitButtonName = "Submit Button";
    private string inputFieldName = "Input Field";
    private Transform storyUI;
    private Transform choicePanel;
    private List<ChoiceOption> choiceOptionList;
    private Transform inputOption;
    private Button inputOptionSubmitButton;
    private TMP_InputField inputField;


    /****** Single tone instance ******/
    public static StoryUIController Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Title UI object
            storyUI = transform.Find(storyUIName);
            if (storyUI == null)
            {
                Debug.Log("Story UI Initialization Error");
                return;
            }
            choicePanel = storyUI.Find(choicePanelName);
            choiceOptionList = new List<ChoiceOption>();
            for(int i = 0; i < choicePanel.childCount - 1; i++)
            {
                choiceOptionList.Add(new ChoiceOption(choicePanel.GetChild(i)));
            }
            inputOption = choicePanel.Find(inputOptionName);
            inputOptionSubmitButton = inputOption.Find(submitButtonName).GetComponent<Button>();
            inputOptionSubmitButton.onClick.AddListener(SubmitInputChoice);
            inputField = inputOption.Find(inputFieldName).GetComponent<TMP_InputField>();
        }
    }


    /****** UI Methods ******/

    // Enter Story UI state
    public void StartUI()
    {
        // Active Story UI object
        storyUI.gameObject.SetActive(true);
    }

    // Update Story UI state
    public void UpdateUI()
    {

    }

    // Exit Story UI state
    public void EndUI()
    {
        // Inactive Story UI object
        choicePanel.gameObject.SetActive(false);
        storyUI.gameObject.SetActive(false);
    }


    public void Attack() { }//PlayNextScript(); }
    public void Submit() { }//PlayNextScript(); }


    // Pause game and show Pause UI
    public void Cancel()
    {
        // Change to Pause UI
        //UIController.Instance.ChangeState(UIController.STATE.PAUSE, false);
    }

    public UIController.STATE GetState()
    {
        return UIController.STATE.STORY;
    }

    public void Move(float move) { return; }
    public void Stop() { return; }



    /****** Show Choice UI on the screen ******/

    // class for choice option
    class ChoiceOption
    {
        Transform optionObject;
        Button optionButton;
        TextMeshProUGUI optionText;
        string branchId;

        public ChoiceOption(Transform optionObject)
        {
            this.optionObject = optionObject;
            optionButton = optionObject.GetComponent<Button>();
            optionButton.onClick.AddListener(OnOptionClick);
            optionText = optionObject.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        // Set option info
        public void SetOption(Option option)
        {
            optionText.text = option.text;
            branchId = option.branchId;
            optionObject.gameObject.SetActive(true);
        }

        // Inactive button object
        public void SetButtonInactive()
        {
            optionObject.gameObject.SetActive(false);
        }

        // Inactive all option buttons and set current story branch according to the branchId of the option
        public void OnOptionClick()
        {
            for (int i = 0; i < Instance.choiceOptionList.Count; i++)
            {
                Instance.choiceOptionList[i].SetButtonInactive();
            }
            Instance.choicePanel.gameObject.SetActive(false);
            StoryModel.Instance.CurrentStoryBranch = branchId; // Change current branchId according to the selected option
        }
    }

    // Show Choice panel on the screen
    public void ShowChoice()
    {
        choicePanel.gameObject.SetActive(true);

        //for(int i = 0; i<choice.options.Count; i++)
        //{
        //    choiceOptionList[i].SetOption(choice.options[i]);
        //}

        inputOption.gameObject.SetActive(true);
    }

    // When player write input choice
    public void SubmitInputChoice()
    {
        // Generate select choice event
        GameEventProducer.Instance.GenerateSelectChoiceEvent(inputField.text);

        // Reset input field
        inputField.text = "";

        // Turn off choice UI
        inputOption.gameObject.SetActive(false);
        choicePanel.gameObject.SetActive(false);
    }

}