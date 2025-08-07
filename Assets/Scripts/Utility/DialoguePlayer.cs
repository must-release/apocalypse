using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// public class DialoguePlayer : MonoBehaviour, IUtilityTool
// {
//     /****** Single tone instance ******/
//     public static DialoguePlayer Instance;


//     /****** Public Fields ******/
//     public List<StoryDialogue> PlayingDialgoueEntries { get; private set; }


//     /****** Private fields ******/
//     private Dictionary<StoryDialogue, Coroutine> dialogueCoroutineMapping; // Used when mapping dialogue entry to playing coroutine
//     private float textInterval = 0.05f; // Speed of the dialogue text


//     /****** Methods ******/
//     public void Awake()
//     {
//         PlayingDialgoueEntries = new List<StoryDialogue>();
//         dialogueCoroutineMapping = new Dictionary<StoryDialogue, Coroutine>();
//     }

//     public void Start()
//     {
//         UtilityManager.Instance.AddUtilityTool(this);
//     }

//     // Stop all dialouges
//     public void ResetTool()
//     {
//         StopAllCoroutines();
//         dialogueCoroutineMapping.Clear();
//         PlayingDialgoueEntries.Clear();
//     }

//     public void PlayDialogue(StoryDialogue dialogue, TMP_Text nameText, TMP_Text dlogText)
//     {
//         nameText.text = dialogue.Name;

//         PlayingDialgoueEntries.Add(dialogue); // Add input entry into the playing entry list
//         dialogueCoroutineMapping[dialogue] = StartCoroutine(TypeSentece(dialogue, dlogText)); // Save entry's coroutine
//     }

//     IEnumerator TypeSentece(StoryDialogue dialogue, TMP_Text dlogText)
//     {
//         dlogText.text = ""; // Initialize Text
//         foreach (char letter in dialogue.Text.ToCharArray())
//         {
//             dlogText.text += letter; // Add letters one by one
//             yield return new WaitForSeconds(textInterval); // Wait before next letter
//         }
//         dialogueCoroutineMapping.Remove(dialogue);
//         PlayingDialgoueEntries.Remove(dialogue);
//     }

//     // Complete dialogue immediately
//     public void CompleteDialogue(StoryDialogue dialogue, TMP_Text nameText, TMP_Text dlogText)
//     {
//         // If dialogue was being played
//         if (dialogueCoroutineMapping.TryGetValue(dialogue, out Coroutine dialogueCoroutine))
//         {
//             StopCoroutine(dialogueCoroutine); // Complete current dialogue
//             dialogueCoroutineMapping.Remove(dialogue);
//             PlayingDialgoueEntries.Remove(dialogue);
//         }

//         // Set total text immediately
//         nameText.text = dialogue.Name;
//         dlogText.text = dialogue.Text; 
//     }
// }

