using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* Part of InputManager which manages Story UI logic */

public class StoryUIState : MonoBehaviour, IUIState, IStoryInfo
{

    /****** Private fields ******/
    private string storyUIName = "Story Player";
    private Transform storyUI;
    private List<StoryEntry> storyLog;
    private Queue<StoryEntry> storyQueue;



    /****** Properties ******/
    public Queue<StoryEntry> StoryQueue
    {
        get { return storyQueue; }
        set
        {
            storyQueue = value;

            // When new story is loaded, start the script automatically
            NextScript();
        }
    }
    public int LastDialogueNum { get; set; } = 0;



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
        }
    }

    /****** Methods ******/

    // Enter Story UI state
    public void StartUI()
    {
        // Active Story UI object
        storyUI.gameObject.SetActive(true);

        // Disable stage objects
        StageManager.Instance.SetStageObjectsActive(false);
    }

    // Update Story UI state
    public void UpdateUI()
    {

    }

    // Exit Story UI state
    public void EndUI()
	{
        // reset storyQueue
        storyQueue = null;

        // Enable stage objects
        StageManager.Instance.SetStageObjectsActive(true);

        // Inactive Story UI object
        storyUI.gameObject.SetActive(false);
    }

    public void Attack() { NextScript(); }
    public void Submit() { 
        // Error when start new game with enter button
        if(storyQueue != null)
            NextScript(); 
    }

    // Story Text is prepared, so start the story
    public void NextScript()
    {
        // Read all story
        if(storyQueue.Count == 0)
        {
            // Reset last dialogue number to 0
            GameManager.Instance.PlayerData.LastDialogueNumber = 0;
            LastDialogueNum = 0;

            EventManager.Instance.EventOver();
            return;
        }

        StoryEntry entry = storyQueue.Dequeue();
        LastDialogueNum++;

        if (entry is Dialogue dialogue)
        {
            Debug.Log($"Dialogue: Character: {dialogue.character}, Text: {dialogue.text}, Branch ID: {dialogue.branchId}");
        }
        else if (entry is Effect effect)
        {
            Debug.Log($"Effect: Action: {effect.action}, Duration: {effect.duration}");
        }
        else if (entry is Choice choice)
        {
            foreach (var option in choice.options)
            {
                Debug.Log($"Choice: Option Text: {option.text}, Branch ID: {option.branchId}");
            }
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
}