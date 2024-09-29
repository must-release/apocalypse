using UnityEngine;

public class Ladder : InteractionObject
{   
    Transform ladderGround;
    float climbUpHeight = 0.1f;

    private void Start() 
    {
        ladderGround = transform.Find("Ground");
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

    public override void StartInteraction(CharacterBase target, ControlInfo controlInfo)
    {
        base.StartInteraction(target, controlInfo);

        // Turn off ground object
        ladderGround.gameObject.SetActive(false);

        // Move character near the ladder
        if(controlInfo.upDown > 0)
        {
            target.transform.position = new Vector3(transform.position.x,
                target.transform.position.y + climbUpHeight, target.transform.position.z);
        }
        else
        {
            target.transform.position = new Vector3(transform.position.x,
                ladderGround.position.y - target.CharacterHeight / 4, target.transform.position.z);
        }
    }

    public override bool CheckInteractingControl(CharacterBase character, ControlInfo controlInfo)
    {
        if (!base.CheckInteractingControl(character, controlInfo)) return false;

        if (character.transform.position.y > ladderGround.position.y || character.StandingGround
            || controlInfo.jump)
        {
            return controlInfo.climb = false;
        }
        else return true;
    }

    public override void EndInteraction(CharacterBase target, ControlInfo controlInfo)
    {
        base.EndInteraction(target, controlInfo);

        // Turn on ground object
        ladderGround.gameObject.SetActive(true);
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