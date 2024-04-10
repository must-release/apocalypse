using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class StoryPlayer : MonoBehaviour
{
    /****** Public Fields ******/
    public List<StoryEntry> PlayingEntries { get; private set; }


    /****** Private fields ******/
    private Dictionary<Dialogue, TMP_Text> dialogueTextMapping; // Used when mapping dialogue entry to target text field
    private float textSpeed = 0.1f; // Speed of the dialogue text


    /****** Single tone instance ******/
    public static StoryPlayer Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PlayingEntries = new List<StoryEntry>();
            dialogueTextMapping = new Dictionary<Dialogue, TMP_Text>();
        }
    }

    // Complete every playing story entry
    public void CompletePlaying()
    {
        // Stop all playing entries
        StopAllCoroutines();

        // Complete every playing entry
        foreach(StoryEntry entry in PlayingEntries)
        {
            if(entry is Dialogue dialogue)
            {
                CompleteDialogue(dialogue);
            }
        }
        PlayingEntries.Clear();
    }


    /******* Show dialogue on the screen ********/

    public bool ShowDialogue(Dialogue dialogue, TMP_Text nameText, TMP_Text dlogText)
    {
        PlayingEntries.Add(dialogue); // Add input entry into the playing entry list
        dialogueTextMapping[dialogue] = dlogText; // Add dialogue - TMP_Text mapping

        nameText.text = dialogue.character;
        StartCoroutine(TypeSentece(dialogue, dlogText));

        return true;
    }

    public IEnumerator TypeSentece(Dialogue dialogue, TMP_Text dlogText)
    {
        dlogText.text = ""; // Initialize Text
        foreach (char letter in dialogue.text.ToCharArray())
        {
            dlogText.text += letter; // Add letters one by one
            yield return new WaitForSeconds(textSpeed); // Wait before next letter
        }
        PlayingEntries.Remove(dialogue);
        dialogueTextMapping.Remove(dialogue);
    }

    // Complete playing dialogue immediately
    public void CompleteDialogue(Dialogue dialogue)
    {
        if (dialogueTextMapping.TryGetValue(dialogue, out TMP_Text dlogText))
        {
            dlogText.text = dialogue.text; // Set total text immediately
            dialogueTextMapping.Remove(dialogue); // 매핑에서 대화 제거
        }
    }


    /******** Play screen effect ********/
    public void PlayEffect(Effect effect)
    {

    }
}

