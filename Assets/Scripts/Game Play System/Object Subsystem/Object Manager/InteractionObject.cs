using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    protected List<CharacterBase> interactableCharacters = new List<CharacterBase>();
    protected List<CharacterBase> interactingCharacters = new List<CharacterBase>();

    // Check if object can be interacted, and set object control info
    public virtual bool CheckInteractableControl(CharacterBase character, ControlInfo controlInfo)
    {
        if (!interactableCharacters.Contains(character))
        {
            Debug.LogError("Checking unknown interactable character");
            return false;
        }
        else return true;
    }

    // Start interaction with target
    public virtual void StartInteraction(CharacterBase target, ControlInfo controlInfo)
    {
        // Change target to interacting object
        interactableCharacters.Remove(target);
        interactingCharacters.Add(target);
    }

    // Check if interaction can be maintained, and set object control info
    public virtual bool CheckInteractingControl(CharacterBase character, ControlInfo controlInfo)
    {
        if (!interactingCharacters.Contains(character))
        {
            Debug.LogError("Checking unknown interacting character");
            return false;
        }
        else return true;
    }

    // End interaction with target
    public virtual void EndInteraction(CharacterBase target, ControlInfo controlInfo)
    {
        if(interactingCharacters.Contains(target))
        {
            // Change target to interactable object
            interactingCharacters.Remove(target);
            interactableCharacters.Add(target);
        }
    }

    // Detect interacting character and recognize each other
    public void DetectCharacterEnter(CharacterBase character)
    {
        bool notInteractable = !interactableCharacters.Contains(character);
        bool notInteracting = !interactingCharacters.Contains(character);
    
        if(notInteractable && notInteracting)
        {
            interactableCharacters.Add(character);
            character.RecognizeInteractionObject(this);
        }
        else
        {
            Debug.LogError("Detecting duplicate character");
        }
    }

    // Detect escaping character and forget each other
    public void DetectCharacterExit(CharacterBase character)   
    {
        bool removedFromInteractable = interactableCharacters.Remove(character);
        bool removedFromInteracting = interactingCharacters.Remove(character);

        if (removedFromInteractable || removedFromInteracting)
        {
            character.ForgetInteractionObject(this);
            EndInteraction(character, null);
        }
        else
        {
            Debug.LogError("Attempted to remove an unknown character.");
        }
    }
}