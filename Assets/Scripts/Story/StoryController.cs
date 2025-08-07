using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class StoryController : MonoBehaviour
{
    /****** Public Members ******/

    public static StoryController Instance;

    [Header("Parameters")]
    public bool IsStoryPlaying { get; private set; }
    public float textSpeed = 0.1f; // Speed of the dialogue text
    public bool isWaitingResponse = false;
    public bool isChatting = false;
    public int responseCount = 0;
    public const int MAX_RESPONSE_COUNT = 4;

    [Header("Assets")]
    public Transform storyScreen;
    public GameObject[] characters;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Coroutine StartStory(string storyInfo, int readBlockCount, int readEntryCount)
    {
        return StartCoroutine(StartStoryCoroutine(storyInfo, readBlockCount, readEntryCount));
    }

    public void FinishStory()
    {
        storyScreen.gameObject.SetActive(false);

        // Give dialogue player back
        UtilityManager.Instance.GiveUtilityBack(_dialoguePlayer);
        _dialoguePlayer = null;
    }


    public void PlayNextScript()
    {
        if (CharacterCGController.Instance.PlayingStandingEntry != null)
        {
            if (CharacterCGController.Instance.PlayingStandingEntry.IsBlockingAnimation)
                return;

            // if Standing Animation is unBlockable, Skip Animation.
            CharacterCGController.Instance.CompleteStandingAnimation();
        }

        if (_dialoguePlayer.PlayingDialgoueEntries.Count > 0)
        {
            // Complete current playing dialogue entry
            StoryDialogue dialogue = _dialoguePlayer.PlayingDialgoueEntries[0];
            _dialoguePlayer.CompleteDialogue(dialogue, nameText, dialogueText);
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

    public void ShowStoryEntry(StoryEntry entry)
    {
        //
        if (entry is StoryDialogue dialogue)
        {
            _dialoguePlayer.PlayDialogue(dialogue, nameText, dialogueText);
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
    public void ProcessSelectedChoice(string optionText)
    {
        // Play selected choice option
        StoryDialogue inputDialogue = new StoryDialogue("나", optionText);
        StoryModel.Instance.StoryEntryBuffer.Enqueue(inputDialogue);
        PlayNextScript();

        StoryModel.Instance.SetCurrentBranch(optionText);
    }

    public void GetStoryProgressInfo(out int readBlockCount, out int readEntryCount)
    {
        readBlockCount = StoryModel.Instance.ReadBlockCount;
        readEntryCount = StoryModel.Instance.ReadEntryCount;
    }


    /****** Private Menbers ******/

    private DialoguePlayer _dialoguePlayer;

    public void Awake()
    {
        Debug.Assert(null != storyScreen, "StoryScreen is not assigned in the editor.");
        Debug.Assert(null != characters, "Character Object is not assigned in the editor.");
        Debug.Assert(null != nameText, "Name Text is not assigned in the editor.");
        Debug.Assert(null != dialogueText, "Dialogue Text is not assigned in the editor.");

        if (Instance == null)
        {
            Instance = this;
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

    private IEnumerator StartStoryCoroutine(string storyInfo, int readBlockCount, int readEntryCount)
    {
        // Set Story Playing true
        IsStoryPlaying = true;

        // Activate Story Screen
        storyScreen.gameObject.SetActive(true);

        // Get dialogue player
        _dialoguePlayer = UtilityManager.Instance.GetUtilityTool<DialoguePlayer>();

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
}

