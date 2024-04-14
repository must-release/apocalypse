using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class StoryPlayer : MonoBehaviour
{
    /****** Single tone instance ******/
    public static StoryPlayer Instance;


    /****** Public Fields ******/
    public List<StoryEntry> PlayingEntries { get; private set; }


    /****** Private fields ******/
    private Dictionary<Dialogue, TMP_Text> dialogueTextMapping; // Used when mapping dialogue entry to target text field
    private Coroutine dialogueCoroutine;
    private float textSpeed = 0.05f; // Speed of the dialogue text

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PlayingEntries = new List<StoryEntry>();
            dialogueTextMapping = new Dictionary<Dialogue, TMP_Text>();
        }
    }

    // Stop all playing story entries
    public void StopPlaying()
    {
        StopAllCoroutines();
        dialogueTextMapping.Clear();
        dialogueCoroutine = null;
        PlayingEntries.Clear();
    }

    /******* Show dialogue on the screen ********/

    public bool ShowDialogue(Dialogue dialogue, TMP_Text nameText, TMP_Text dlogText)
    {
        PlayingEntries.Add(dialogue); // Add input entry into the playing entry list
        dialogueTextMapping[dialogue] = dlogText; // Add dialogue - TMP_Text mapping

        nameText.text = dialogue.character;
        dialogueCoroutine = StartCoroutine(TypeSentece(dialogue, dlogText));

        return true;
    }

    IEnumerator TypeSentece(Dialogue dialogue, TMP_Text dlogText)
    {
        dlogText.text = ""; // Initialize Text
        foreach (char letter in dialogue.text.ToCharArray())
        {
            dlogText.text += letter; // Add letters one by one
            yield return new WaitForSeconds(textSpeed); // Wait before next letter
        }
        PlayingEntries.Remove(dialogue);
        dialogueTextMapping.Remove(dialogue);
        dialogueCoroutine = null;
    }

    // Complete first playing dialogue immediately
    public void CompleteDialogue()
    {
        Dialogue dialogue = null;

        // Chech if there is only dialogue entry in the list
        foreach (StoryEntry entry in PlayingEntries)
        {
            if (entry is Dialogue dlog)
            {
                dialogue = dlog;
            }
            else
            {
                return;
            }
        }

        if (dialogueTextMapping.TryGetValue(dialogue, out TMP_Text dlogText))
        {
            StopCoroutine(dialogueCoroutine); // Complete current dialogue
            dlogText.text = dialogue.text; // Set total text immediately
            PlayingEntries.Remove(dialogue);
            dialogueTextMapping.Remove(dialogue);
        }
    }


    /******** Play screen effect ********/
    public void PlayEffect(Effect effect)
    {

    }
}

