﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class StoryController : MonoBehaviour
{
    public static StoryController Instance;

    public bool IsStoryPlaying { get; private set; }
    public float textSpeed = 0.1f; // Speed of the dialogue text
    public bool isWaitingResponse = false;
    public bool isChatting = false;
    public int responseCount = 0;
    public const int MAX_RESPONSE_COUNT = 4;

    private string storyScreenName = "Story Screen";
    private string characterName = "Character";
    private string dialogueBoxName = "Dialogue Box";
    private string nameTextName = "Name Text";
    private string dialogueTextName = "Dialogue Text";
    private Transform storyScreen;
    private GameObject character;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    private DialoguePlayer dialoguePlayer;

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

    // Start Story Mode with given info
    public Coroutine StartStory(string storyInfo, int readBlockCount, int readEntryCount)
    {
        return StartCoroutine(StartStoryCoroutine(storyInfo, readBlockCount, readEntryCount));
    }
    IEnumerator StartStoryCoroutine(string storyInfo, int readBlockCount, int readEntryCount)
    {
        // Set Story Playing true
        IsStoryPlaying = true;

        // Activate Story Screen
        storyScreen.gameObject.SetActive(true);

        // Get dialogue player
        dialoguePlayer = UtilityManager.Instance.GetUtilityTool<DialoguePlayer>();

        // Load Story Text according to the Info
        yield return StoryModel.Instance.LoadStoryText(storyInfo, readBlockCount, readEntryCount);

        // Show first story script When new story is loaded
        StoryEntry entry = StoryModel.Instance.GetFirstEntry();
        if (entry == null)
        {
            Debug.Log("Story initial load error");
            yield break;
        }
        ShowStoryEntry(entry);
    }

    // Finish Story Mode
    public void FinishStory()
    {
        // Inactivate Story Screen
        storyScreen.gameObject.SetActive(false);

        // Give dialogue player back
        UtilityManager.Instance.GiveUtilityBack(dialoguePlayer);
        dialoguePlayer = null;

        responseCount = 0;
        //MemoryAPI.Instance.Reflect();
    }


    // Play next script on the screen
    public void PlayNextScript()
    {
        if (dialoguePlayer.PlayingDialgoueEntries.Count > 0)
        {
            if (dialoguePlayer.PlayingDialgoueEntries.Count > 1)
            {
                Debug.Log("Error: multiple dialogue at the list");
            }

            // Complete current playing dialogue entry
            Dialogue dialogue = dialoguePlayer.PlayingDialgoueEntries[0];
            dialoguePlayer.CompleteDialogue(dialogue, nameText, dialogueText);
        }
        else
        {
            if (isWaitingResponse) // Waiting for AI's response
            {
                nameText.text = "";
                dialogueText.text = "Loading";
            }
            else if (isChatting && StoryModel.Instance.StoryEntryBuffer.Count == 0) // Continue chating
            {
                GameEventProducer.Instance.GenerateChoiceEventStream();
            }
            else
            {
                // Check if there is available entry
                StoryEntry entry = StoryModel.Instance.GetNextEntry();
                if (entry == null)
                {
                    // Story is over
                    IsStoryPlaying = false;
                }
                else
                {
                    // Show Story Entry
                    ShowStoryEntry(entry);
                }
            }
        }
    }

    // Show story entry on the screen
    public void ShowStoryEntry(StoryEntry entry)
    {
        if (entry is Dialogue dialogue)
        {
            dialoguePlayer.PlayDialogue(dialogue, nameText, dialogueText);
            //MemoryAPI.Instance.SaveMemory(dialogue);
        }
        else if (entry is Choice choice)
        {
            // Save processing choice
            StoryModel.Instance.ProcessingChoice = choice;

            // Extracting the text values from the options
            List<string> optionTexts = choice.options.Select(option => option.text).ToList();

            // Generate show choice event stream
            GameEventProducer.Instance.GenerateChoiceEventStream(optionTexts);
        }
        else
        {
            Debug.Log("story entry error: no such entry");
        }
    }


    // Process selected choice
    public void ProcessSelectedChoice(string optionText, bool generateResponse)
    {
        // Play selected choice option
        Dialogue inputDialogue = new Dialogue("나", optionText);
        StoryModel.Instance.StoryEntryBuffer.Enqueue(inputDialogue);
        PlayNextScript();

        if (generateResponse) // Generate response of AI
        {
            StartCoroutine(GetResponse(inputDialogue));
        }
        else // Set current branch info
        {
            StoryModel.Instance.SetCurrentBranch(optionText);
        }
    }

    // Get response of the AI Character
    IEnumerator GetResponse(Dialogue inputDialogue)
    {
        // Set response settings
        isWaitingResponse = true;
        responseCount++;
        if (responseCount >= MAX_RESPONSE_COUNT) isChatting = false;
        else isChatting = true;

        // Get response
        yield return MemoryAPI.Instance.GenerateResponse(inputDialogue, isChatting);
        string response = MemoryAPI.Instance.Response;
        isWaitingResponse = false;

        // Split the string by newline characters and remove empty entries
        string[] lines = response.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        // Convert the array to a list
        var linesList = lines.ToList();

        foreach (var line in linesList)
        {
            // Skip empty lines, just in case
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Prepare response
            Dialogue responseDialogue = new Dialogue("연아", line);
            StoryModel.Instance.StoryEntryBuffer.Enqueue(responseDialogue);
        }

        // If dialogue text is loading, auto play response
        if (dialogueText.text.Equals("Loading"))
        {
            PlayNextScript();
        }
    }

    // Get current story progress info
    public (int, int) GetStoryProgressInfo()
    {
        return (StoryModel.Instance.ReadBlockCount, StoryModel.Instance.ReadEntryCount);
    }
}
