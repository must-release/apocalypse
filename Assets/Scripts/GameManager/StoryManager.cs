using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
	public static StoryManager Instance { get; private set; }

    private List<StoryObserver> observerList; // Observers which observes story queue
    private List<StoryEntry> storyLog;
    private Queue<StoryEntry> storyQueue;


    /****** Properties ******/
    public Queue<StoryEntry> StoryQueue
    {
        get { return storyQueue; }
        set
        {
            storyQueue = value;

            // Notify observers about the update
            if(storyQueue != null)
            {
                observerList.ForEach((obj) => obj.Update());
            }
        }
    }
    public int LastDialogueNum { get; set; } = 0;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            observerList = new List<StoryObserver>();
        }
    }

    // Add new observer to the list
    public void AddObserver(StoryObserver newObserver)
    {
        observerList.Add(newObserver);
    }

    public void SetStoryDetails()
    {

    }
}

public interface StoryObserver
{
    public void Update();
}