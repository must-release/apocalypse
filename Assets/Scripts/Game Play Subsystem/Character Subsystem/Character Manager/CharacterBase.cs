using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    public GameObject StandingGround {get; protected set;} 
    public float CharacterHeight {get; protected set;}

    protected List<InteractionObject> interactableObjects = new List<InteractionObject>();
    protected List<InteractionObject> interactingObjects = new List<InteractionObject>();


    /***** Abstract Functions *****/
    public abstract void ControlCharacter(ControlInfo controlInfo);
    public abstract void OnAir();
    public abstract void OnGround();
    public abstract void OnDamaged();

    private void Awake() { AwakeCharacter(); }
    protected virtual void AwakeCharacter()
    {
        gameObject.layer = LayerMask.NameToLayer("Character");
    }

    private void Start() { StartCharacter(); }
    protected virtual void StartCharacter() { }


    /***** Object Interaction Functions *****/
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


    /********** Detect Collision **********/
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if(collision.gameObject == StandingGround)
            {
                StandingGround = null;
                if(gameObject.activeSelf)
                    StartCoroutine(OnAirDelay());
            }
        }
    }
    IEnumerator OnAirDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if(StandingGround == null) OnAir();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Ground")) && StandingOnGround(collision))
        {
            OnGround();
            StandingGround = collision.gameObject;
        }
    } 

    private bool StandingOnGround(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        float angle = Vector2.Angle(normal, Vector2.up);
        if(angle > 45) return false;
        else return true;
    }
}