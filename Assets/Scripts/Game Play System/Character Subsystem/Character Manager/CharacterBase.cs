using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    public GameObject StandingGround {get; protected set;} 
    public float CharacterHeight {get; protected set;}

    protected List<InteractionObject> interactableObjects = new List<InteractionObject>();
    protected List<InteractionObject> interactingObjects = new List<InteractionObject>();
    public abstract void ControlCharacter(ControlInfo controlInfo);

    public void RecognizeInteractionObject(InteractionObject obj)
    {
        bool notInteractable = !interactableObjects.Contains(obj);
        bool notInteracting = !interactingObjects.Contains(obj);

        if(notInteractable && notInteracting)
        {
            interactableObjects.Add(obj);
        }
        else
        {
            Debug.LogError("Detecting duplicate Object");
        }
    }
    public void ForgetInteractionObject(InteractionObject obj)
    {
        bool removedFromInteractable = interactableObjects.Remove(obj);
        bool removedFromInteracting = interactingObjects.Remove(obj);

        if (!removedFromInteractable && !removedFromInteracting)
        {
            Debug.LogError("Removing unknown Object");
        }
    }

    // Control Interaction objects and set object control info
    protected void ControlInteractionObjects(ControlInfo controlInfo)
    {
        // Control interacting objects
        for (int i = interactingObjects.Count - 1; i >= 0; i--)
        {
            if(!interactingObjects[i].CheckInteractingControl(this, controlInfo))
            {
                interactingObjects[i].EndInteraction(this, controlInfo);
                interactableObjects.Add(interactingObjects[i]);
                interactingObjects.RemoveAt(i);
            }
        }

        // Control interactable objects
        for (int i = interactableObjects.Count - 1; i >= 0; i--)
        {
            if(interactableObjects[i].CheckInteractableControl(this, controlInfo))
            {
                interactableObjects[i].StartInteraction(this, controlInfo);
                interactingObjects.Add(interactableObjects[i]);
                interactableObjects.RemoveAt(i);
            }
        }
    }
}