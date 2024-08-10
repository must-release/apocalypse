using UnityEngine;
using UIEnums;

/* Part of UIController which manages Story UI logic */

public class StoryUIController : MonoBehaviour, IUIController
{

    /****** Private fields ******/
    private string storyUIName = "Story UI";
    private Transform storyUI;


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
        }
    }

    public void Start()
    {
        // Add current UI controller
        UIController.Instance.AddUIController(BASEUI.STORY, Instance);
    }

    /****** UI Methods ******/

    // Enter Story UI
    public void StartUI()
    {
        // Active Story UI object
        storyUI.gameObject.SetActive(true);
    }


    // Update Story UI
    public void UpdateUI()
    {

    }

    // Exit Story UI
    public void EndUI()
    {
        // Inactive Story UI object
        storyUI.gameObject.SetActive(false);
    }

    public void Cancel()
    {

    }
}