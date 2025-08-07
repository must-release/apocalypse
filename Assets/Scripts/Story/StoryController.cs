using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class StoryController : MonoBehaviour
{
    public static StoryController Instance;

    [Header("Parameters")]
    public bool IsStoryPlaying { get; private set; }
    public float textSpeed = 0.1f; // Speed of the dialogue text
    public bool isWaitingResponse = false;
    public bool isChatting = false;
    public int responseCount = 0;
    public const int MAX_RESPONSE_COUNT = 4;

    // private string storyScreenName = "Story Screen";
    // private string characterName = "Character";
    // private string dialogueBoxName = "Dialogue Box";
    // private string nameTextName = "Name Text";
    // private string dialogueTextName = "Dialogue Text";

    [Header("Assets")]
    public Transform storyScreen;
    public GameObject[] characters;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private DialoguePlayer dialoguePlayer;

    public void Awake()
    {
        Debug.Assert(null != storyScreen, "StoryScreen is not assigned in the editor.");
        Debug.Assert(null != characters, "Character Object is not assigned in the editor.");
        Debug.Assert(null != nameText, "Name Text is not assigned in the editor.");
        Debug.Assert(null != dialogueText, "Dialogue Text is not assigned in the editor.");

        if (Instance == null)
        {
            Instance = this;

            // Find Story Screen object
            // storyScreen = transform.Find(storyScreenName);
            // if (storyScreen == null)
            // {
            //     Debug.Log("Story UI Initialization Error");
            //     return;
            // }

            // character = storyScreen.Find(characterName).gameObject;
            // nameText = storyScreen.Find(dialogueBoxName).Find(nameTextName).GetComponent<TextMeshProUGUI>();
            // dialogueText = storyScreen.Find(dialogueBoxName).Find(dialogueTextName).GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        var canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        foreach (var character in characters)
        {
            var view = character.GetComponent<CharacterCGView>();
            if (view != null)
            {
                CharacterCGController.Instance.RegisterCharacter(view);
            }

            // blinding
            character.SetActive(false);
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
        if (CharacterCGController.Instance.PlayingStandingEntry != null)
        {
            if (CharacterCGController.Instance.PlayingStandingEntry.IsBlockingAnimation)
                return;

            // if Standing Animation is unBlockable, Skip Animation.
            CharacterCGController.Instance.CompleteStandingAnimation();
        }

        if (dialoguePlayer.PlayingDialgoueEntries.Count > 0)
        {
            if (dialoguePlayer.PlayingDialgoueEntries.Count > 1)
            {
                Debug.Log("Error: multiple dialogue at the list");
            }

            // Complete current playing dialogue entry
            StoryDialogue dialogue = dialoguePlayer.PlayingDialgoueEntries[0];
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
                //GameEventProducer.Instance.GenerateChoiceEventStream();
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
        //
        if (entry is StoryDialogue dialogue)
        {
            dialoguePlayer.PlayDialogue(dialogue, nameText, dialogueText);
            //MemoryAPI.Instance.SaveMemory(dialogue);
        }
        else if (entry is StoryChoice choice)
        {
            // Save processing choice
            StoryModel.Instance.ProcessingChoice = choice;

            // Extracting the text values from the options
            List<string> optionTexts = choice.Options.Select(option => option.Text).ToList();

            // Generate show choice event
            var choiceEvent = GameEventFactory.CreateChoiceEvent(optionTexts);
            GameEventManager.Instance.Submit(choiceEvent);


        }
        else if (entry is StoryCharacterCG standing)
        {
            CharacterCGController.Instance.HandleCharacterCG(standing);
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
        StoryDialogue inputDialogue = new StoryDialogue("나", optionText);
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
    IEnumerator GetResponse(StoryDialogue inputDialogue)
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
            StoryDialogue responseDialogue = new StoryDialogue("연아", line);
            StoryModel.Instance.StoryEntryBuffer.Enqueue(responseDialogue);
        }

        // If dialogue text is loading, auto play response
        if (dialogueText.text.Equals("Loading"))
        {
            PlayNextScript();
        }
    }

    public void GetStoryProgressInfo(out int readBlockCount, out int readEntryCount)
    {
        readBlockCount = StoryModel.Instance.ReadBlockCount;
        readEntryCount = StoryModel.Instance.ReadEntryCount;
    }
}

