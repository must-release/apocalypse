using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class StoryController : MonoBehaviour
{
    public static StoryController Instance;

    public bool IsStoryPlaying { get; private set; } = false;
    public float textSpeed = 0.1f; // Speed of the dialogue text
    public bool isWaitingResponse = false;
    public bool isRegenerate = false;
    public int responseCount = 0;

    private string storyScreenName = "Story Screen";
    private string characterName = "Character";
    private string dialogueBoxName = "Dialogue Box";
    private string nameTextName = "Name Text";
    private string dialogueTextName = "Dialogue Text";
    private Transform storyScreen;
    private GameObject character;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    private Transform choicePanel;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Story Screen object
            storyScreen = transform.Find(storyScreenName);
            if (storyScreen == null)
            {
                Debug.Log("Story UI Initialization Error");
                return;
            }
            character = storyScreen.Find(characterName).gameObject;
            nameText = storyScreen.Find(dialogueBoxName).Find(nameTextName).GetComponent<TextMeshProUGUI>();
            dialogueText = storyScreen.Find(dialogueBoxName).Find(dialogueTextName).GetComponent<TextMeshProUGUI>();
        }
    }

    public void Update()
    {
        if(StoryModel.Instance.storyEntryBuffer.Count > 0)
        {
            if(dialogueText.text == "Loading")
            {
                PlayNextScript();
            }
        }
    }

    // Start Story Mode with given info
    public void StartStory(string storyInfo, int readBlockCount, int readEntryCount)
    {
        // Set Story Playing true
        IsStoryPlaying = true;

        // Activate Story Screen
        storyScreen.gameObject.SetActive(true);

        // Load Story Text according to the Info
        StoryModel.Instance.LoadStoryText(storyInfo, readBlockCount, readEntryCount);
    }

    // Show first story script When new story is loaded
    public void StartScript()
    {
        StoryEntry entry = StoryModel.Instance.GetFirstEntry();
        if (entry == null)
        {
            Debug.Log("Story initial load error");
            return;
        }
        ShowStoryEntry(entry);
    }

    // Finish Story Mode
    public void FinishStory()
    {
        // Inactivate Story Screen
        storyScreen.gameObject.SetActive(false);
        responseCount = 0;
        MemoryAPI.Instance.Reflect();
    }


    // Play next script on the screen
    public void PlayNextScript()
    {
        if (isWaitingResponse)
        {
            nameText.text = "";
            dialogueText.text = "Loading";
            return;
        }

        if (isRegenerate && StoryModel.Instance.storyEntryBuffer.Count == 0)
        {
            GameEventProducer.Instance.GenerateShowChoiceEvent();
            return;
        }

        // Check If StoryPlayer is now playing non-dialogue entry
        if (DialoguePlayer.Instance.NonDialogueEntryCount > 0)
        {
            return; // Wait for non-dialogue entry to end
        }
        else if (DialoguePlayer.Instance.PlayingDialgoueEntries.Count > 0)
        {
            if (DialoguePlayer.Instance.PlayingDialgoueEntries.Count > 1)
            {
                Debug.Log("Error: multiple dialogue at the list");
                return;
            }

            // Complete current playing dialogue entry
            Dialogue dialogue = DialoguePlayer.Instance.PlayingDialgoueEntries[0];
            DialoguePlayer.Instance.CompleteDialogue(dialogue, nameText, dialogueText);
        }
        else
        {

            // Check if there is available entry
            StoryEntry entry = StoryModel.Instance.GetNextEntry();
            if (entry == null)
            {
                IsStoryPlaying = false;
            }

            ShowStoryEntry(entry);
        }
    }

    // Apply selected choice
    public void ApplySelectedChoice(string optionText)
    {
        Dialogue inputDialogue = new Dialogue();
        inputDialogue.character = "나";
        inputDialogue.text = optionText;
        DialoguePlayer.Instance.PlayDialogue(inputDialogue, nameText, dialogueText);

        GetResponse(inputDialogue);
    }

    public void GetResponse(Dialogue inputDialogue)
    {
        isWaitingResponse = true;
        responseCount++;
        MemoryAPI.Instance.SaveMemory(inputDialogue);
        MemoryAPI.Instance.ResponseMemory(inputDialogue, responseCount);
    }

    public void ShowResponse(string response, bool regenerate)
    {
        isWaitingResponse = false;
        isRegenerate = regenerate;

        // Split the string by newline characters
        string[] lines = response.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        // Convert the array to a list
        var linesList = lines.ToList();

        foreach (var line in linesList)
        {
            Dialogue responseDialogue = new Dialogue();
            responseDialogue.character = "연아";
            responseDialogue.text = line;

            StoryModel.Instance.storyEntryBuffer.Enqueue(responseDialogue);
        }
    }

    // Show story entry on the screen
    public void ShowStoryEntry(StoryEntry entry)
    {
        if (entry is Dialogue dialogue)
        {
            DialoguePlayer.Instance.PlayDialogue(dialogue, nameText, dialogueText);
            MemoryAPI.Instance.SaveMemory(dialogue);
        }
        else if (entry is Choice choice)
        {
            // Generate show choice event stream
            GameEventProducer.Instance.GenerateShowChoiceEvent();
        }
        else if (entry is Effect effect)
        {
            DialoguePlayer.Instance.PlayEffect(effect);
            Debug.Log($"Effect: Action: {effect.action}, Duration: {effect.duration}");
        }
        else
        {
            Debug.Log("story entry error: no such entry");
        }
    }
}

