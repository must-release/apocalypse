using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/* Part of InputManager which manages Story UI logic */

public class StoryUIState : MonoBehaviour, IUIState, StoryObserver
{

    /****** Private fields ******/
    private string storyUIName = "Story UI";
    private string characterName = "Character";
    private string dialogueBoxName = "Dialogue Box";
    private string nameTextName = "Name Text";
    private string dialogueTextName = "Dialogue Text";
    private string choicePanelName = "Choice Panel";
    private Transform storyUI;
    private GameObject character;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    private Transform choicePanel;
    private List<ChoiceOption> choiceOptionList;
    public float textSpeed = 0.1f; // Speed of the dialogue text


    /****** Single tone instance ******/
    public static StoryUIState Instance;

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
            character = storyUI.Find(characterName).gameObject;
            nameText = storyUI.Find(dialogueBoxName).Find(nameTextName).GetComponent<TextMeshProUGUI>();
            dialogueText = storyUI.Find(dialogueBoxName).Find(dialogueTextName).GetComponent<TextMeshProUGUI>();
            choicePanel = storyUI.Find(choicePanelName);
            choiceOptionList = new List<ChoiceOption>();
            for(int i = 0; i < choicePanel.childCount; i++)
            {
                choiceOptionList.Add(new ChoiceOption(choicePanel.GetChild(i)));
            }
        }
    }

    public void Start()
    {
        // Add this object as an observer of the story manager
        StoryManager.Instance.AddObserver(this);
    }


    /****** UI Methods ******/

    // Enter Story UI state
    public void StartUI()
    {
        // Active Story UI object
        storyUI.gameObject.SetActive(true);

        // Disable stage objects
        GameSceneManager.Instance.SetStageObjectsActive(false);
    }

    // Update Story UI state
    public void UpdateUI()
    {

    }

    // Exit Story UI state
    public void EndUI()
    {
        // Enable stage objects
        GameSceneManager.Instance.SetStageObjectsActive(true);

        // Inactive Story UI object
        choicePanel.gameObject.SetActive(false);
        storyUI.gameObject.SetActive(false);
    }

    // Show first story script When new story is loaded
    public void StoryUpdated()
    {
        StoryEntry entry = StoryManager.Instance.GetFirstEntry();
        if (entry == null)
        {
            Debug.Log("Story initial load error");
            return;
        }
        ShowStoryEntry(entry);
    }

    public void Attack() { PlayNextScript(); }
    public void Submit() { PlayNextScript(); }


    // Play next script on the screen
    public void PlayNextScript()
    {
        // Check If StoryPlayer is now playing non-dialogue entry
        if(StoryPlayer.Instance.NonDialogueEntryCount > 0)
        {
            return; // Wait for non-dialogue entry to end
        }
        else if (StoryPlayer.Instance.PlayingDialgoueEntries.Count > 0)
        {
            if(StoryPlayer.Instance.PlayingDialgoueEntries.Count > 1)
            {
                Debug.Log("Error: multiple dialogue at the list");
                return;
            }

            // Complete current playing dialogue entry
            Dialogue dialogue = StoryPlayer.Instance.PlayingDialgoueEntries[0];
            StoryPlayer.Instance.CompleteDialogue(dialogue, nameText, dialogueText);
        }
        else
        {
            // Check if there is available entry
            StoryEntry entry = StoryManager.Instance.GetNextEntry();
            if (entry == null) return;

            ShowStoryEntry(entry);
        }
    }

    // Show story entry on the screen
    public void ShowStoryEntry(StoryEntry entry)
    {
        if (entry is Dialogue dialogue)
        {
            StoryPlayer.Instance.PlayDialogue(dialogue, nameText, dialogueText);
        }
        else if (entry is Choice choice)
        {
            // Show choice modal
            ShowChoice(choice);
        }
        else if (entry is Effect effect)
        {
            StoryPlayer.Instance.PlayEffect(effect);
            Debug.Log($"Effect: Action: {effect.action}, Duration: {effect.duration}");
        }
        else
        {
            Debug.Log("story entry error: no such entry");
        }
    }


    // Pause game and show Pause UI
    public void Cancel()
    {
        // Change to Pause UI
        UIManager.Instance.ChangeState(UIManager.STATE.PAUSE, false);
    }

    public UIManager.STATE GetState()
    {
        return UIManager.STATE.STORY;
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
            StoryManager.Instance.CurrentStoryBranch = branchId; // Change current branchId according to the selected option
            Instance.PlayNextScript();
        }
    }

    // Show Choice panel on the screen
    public void ShowChoice(Choice choice)
    {
        choicePanel.gameObject.SetActive(true);

        for(int i = 0; i<choice.options.Count; i++)
        {
            choiceOptionList[i].SetOption(choice.options[i]);
        }
    }

}