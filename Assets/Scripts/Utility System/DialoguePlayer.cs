using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DialoguePlayer : MonoBehaviour
{
    /****** Single tone instance ******/
    public static DialoguePlayer Instance;


    /****** Public Fields ******/
    public List<Dialogue> PlayingDialgoueEntries { get; private set; }
    public int NonDialogueEntryCount { get; private set; } = 0;


    /****** Private fields ******/
    private Dictionary<Dialogue, Coroutine> dialogueCoroutineMapping; // Used when mapping dialogue entry to playing coroutine
    private float textInterval = 0.05f; // Speed of the dialogue text

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PlayingDialgoueEntries = new List<Dialogue>();
            dialogueCoroutineMapping = new Dictionary<Dialogue, Coroutine>();
        }
    }

    // Stop all playing story entries
    public void StopPlaying()
    {
        StopAllCoroutines();
        dialogueCoroutineMapping.Clear();
        PlayingDialgoueEntries.Clear();
    }

    /******* Show dialogue on the screen ********/

    public void PlayDialogue(Dialogue dialogue, TMP_Text nameText, TMP_Text dlogText)
    {
        nameText.text = dialogue.character;

        PlayingDialgoueEntries.Add(dialogue); // Add input entry into the playing entry list
        dialogueCoroutineMapping[dialogue] = StartCoroutine(TypeSentece(dialogue, dlogText)); // Save entry's coroutine
    }

    IEnumerator TypeSentece(Dialogue dialogue, TMP_Text dlogText)
    {
        dlogText.text = ""; // Initialize Text
        foreach (char letter in dialogue.text.ToCharArray())
        {
            dlogText.text += letter; // Add letters one by one
            yield return new WaitForSeconds(textInterval); // Wait before next letter
        }
        dialogueCoroutineMapping.Remove(dialogue);
        PlayingDialgoueEntries.Remove(dialogue);
    }

    // Complete dialogue immediately
    public void CompleteDialogue(Dialogue dialogue, TMP_Text nameText, TMP_Text dlogText)
    {
        // If dialogue was being played
        if (dialogueCoroutineMapping.TryGetValue(dialogue, out Coroutine dialogueCoroutine))
        {
            StopCoroutine(dialogueCoroutine); // Complete current dialogue
            dialogueCoroutineMapping.Remove(dialogue);
            PlayingDialgoueEntries.Remove(dialogue);
        }

        // Set total text immediately
        nameText.text = dialogue.character;
        dlogText.text = dialogue.text; 
    }


    /******** Play screen effect ********/
    public void PlayEffect(Effect effect)
    {
        NonDialogueEntryCount++;
        StartCoroutine(PlayingEffect());
    }

    IEnumerator PlayingEffect()
    {

        Debug.Log("Playing effect");

        yield return new WaitForSeconds(1f);

        Debug.Log("Effect over");

        NonDialogueEntryCount--;
    }

}

