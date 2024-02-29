using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* Part of InputManager which manages Story UI logic */

public class StoryUIState : MonoBehaviour, IUIState
{
    private string storyUIName = "Story UI";
    private static Transform storyUI;

    // UI Initialization
    public void StartUI()
    {
        // Operate only once
        if (storyUI == null)
        {
            // Find Title UI object
            storyUI = FindObjectOfType<Canvas>().transform.Find(storyUIName);
            if (storyUI == null)
            {
                Debug.Log("Story UI Initialization Error");
                return;
            }


        }

        // Active Story UI object
        storyUI.gameObject.SetActive(true);
    }

    public void EndUI()
	{
        // Inactive Story UI object
        storyUI.gameObject.SetActive(false);
    }

	public void Move(float move)
	{

	}
}

