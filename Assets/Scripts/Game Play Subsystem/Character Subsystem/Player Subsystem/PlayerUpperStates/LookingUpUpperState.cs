using UnityEngine;
using CharacterEums;

public class LookingUpUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;

    // Start is called before the first frame update

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(PLAYER_UPPER_STATE.LOOKING_UP, this);
    }

    public PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.LOOKING_UP; }

    public void StartState()
    {
        // Rotate upper body by 90 degree
        playerController.CurrentPlayer.RotateUpperBody(90);
    }
    public void UpdateState()
    {

    }
    public void EndState(PLAYER_UPPER_STATE nextState)
    {
        // Recover upper body rotation when not attacking
        if (nextState != PLAYER_UPPER_STATE.TOP_ATTACKING)
            playerController.CurrentPlayer.RotateUpperBody(0);
    }

    public void Disable() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.DISABLED); }

    public void LookUp(bool lookUp) 
    { 
        if(!lookUp) 
        {
            if (playerController.StandingGround == null)
                playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING);
            else
                playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE);
        }
    }
    public void Aim(Vector3 aim) 
    {  
        if (aim != Vector3.zero && playerController.StandingGround != null)
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.AIMING);
    }

    public void Attack() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.TOP_ATTACKING); }


    /***** Inavailable State Change *****/
    public void OnGround() { return; }
    public void Stop() { return; }
    public void Move() { return; }
    public void Jump() { return; }
    public void OnAir() { return; }
    public void Enable() {return;}
}
