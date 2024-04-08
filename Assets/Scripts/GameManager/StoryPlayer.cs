using UnityEngine;
using System.Collections;
using TMPro;

public class StoryPlayer : MonoBehaviour, StoryObserver
{
    /****** Private fields ******/
    private string storyUIName = "Story UI";
    private string characterName = "Character";
    private string dialogueBoxName = "Dialogue Box";
    private string nameTextName = "Name Text";
    private string dialogueTextName = "Dialogue Text";
    private Transform storyUI;
    private GameObject character;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    public float textSpeed = 0.1f; // Speed of the dialogue text



    /****** Single tone instance ******/
    public static StoryPlayer Instance;

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
        }
    }

    public void Start()
    {
        // Add this object as an observer of the story manager
        StoryManager.Instance.AddObserver(this);
    }

    // Show first story script When new story is loaded
    public void StoryUpdated()
    {
        StoryEntry entry = StoryManager.Instance.GetNextEntry();
        if(entry == null)
        {
            Debug.Log("Story Load error");
            return;
        }
        ShowStoryEntry(entry);
    }

    // Show story entry on the screen
    public void ShowStoryEntry(StoryEntry entry)
    {
        if (entry is Dialogue dialogue)
        {
            ShowDialogue(dialogue);
        }
        else if (entry is Choice choice)
        {

            foreach (var option in choice.options)
            {
                Debug.Log($"Choice: Option Text: {option.text}, Branch ID: {option.branchId}");
            }
        }
        else if (entry is Effect effect)
        {
            PlayEffect(effect);
            Debug.Log($"Effect: Action: {effect.action}, Duration: {effect.duration}");
        }
    }


    /******* Show dialogue on the screen ********/

    public void ShowDialogue(Dialogue dialogue)
    {
        nameText.text = dialogue.character;
        StartCoroutine(TypeSentece(dialogue.text));
    }

    public IEnumerator TypeSentece(string sentence)
    {
        dialogueText.text = ""; // Initialize Text
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter; // Add letters one by one
            yield return new WaitForSeconds(textSpeed); // Wait before next letter
        }
    }


    /****** Show Choice UI on the screen ******/
    public void ShowChoice(Choice UI)
    {

    }



    /******** Play screen effect ********/
    public void PlayEffect(Effect effect)
    {

    }
}

