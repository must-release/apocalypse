using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StoryUIState : MonoBehaviour, IUIState
{
    private string storyUIName = "Story UI";
    private static Transform storyUI;

    private List<DialogueEntry> DialogueList;

    // UI Initialization
    public void StartUI()
    {
        if (storyUI == null)
        {
            storyUI = FindObjectOfType<Canvas>().transform.Find(storyUIName);
            if (storyUI == null)
            {
                Debug.Log("Story UI Initialization Error");
                return;
            }
        }

        storyUI.gameObject.SetActive(true);

    }

    public void EndUI()
	{
        storyUI.gameObject.SetActive(false);
    }

	public void Move(float move)
	{

	}
}

