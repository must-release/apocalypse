using UnityEngine;

public class Ladder : InteractionObject
{   
    Transform ladderGround;
    Transform bottom;

    private void Start() 
    {
        ladderGround = transform.Find("Ground");
        bottom = transform.Find("Bottom");
    }

    // Check if ladder object can be interacted, and set object control info
    public override bool CheckInteractableControl(CharacterBase character, ControlInfo controlInfo)
    {
        if (!base.CheckInteractableControl(character, controlInfo)) return false;

        if (character.transform.position.y > ladderGround.position.y) // if character is standing over the ladder
        {
            return controlInfo.climb = controlInfo.upDown < 0;
        }
        else // if character is standing under the ladder
        { 
            return controlInfo.climb = controlInfo.upDown > 0;
        }
    }

    // Start interaction between character and ladder
    public override void StartInteraction(CharacterBase target, ControlInfo controlInfo)
    {
        base.StartInteraction(target, controlInfo);

        // Ignore collision between ladder ground and character
        Physics2D.IgnoreCollision(target.GetComponent<Collider2D>(), 
            ladderGround.GetComponent<Collider2D>(), true);

        // Set current ladder as a climbing object
        controlInfo.climbingObject = gameObject;
    }

    // Check if ladder object and character can still interact, and set object control info
    public override bool CheckInteractingControl(CharacterBase character, ControlInfo controlInfo)
    {
        if (!base.CheckInteractingControl(character, controlInfo)) return false;

        if (character.transform.position.y > ladderGround.position.y || 
            character.transform.position.y < bottom.position.y ||
            character.StandingGround || controlInfo.jump)
        {
            return controlInfo.climb = false;
        }
        else return true;
    }

    // End interaction between character and ladder
    public override void EndInteraction(CharacterBase target, ControlInfo controlInfo)
    {
        base.EndInteraction(target, controlInfo);

        // Enable collision check between ladder ground and character
        Physics2D.IgnoreCollision(target.GetComponent<Collider2D>(), 
            ladderGround.GetComponent<Collider2D>(), false);
    }


    /******** Detect Character *********/
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.TryGetComponent(out CharacterBase character))
        {
            DetectCharacterEnter(character);
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.TryGetComponent(out CharacterBase character))
        {
            DetectCharacterExit(character);
        }
    }
}