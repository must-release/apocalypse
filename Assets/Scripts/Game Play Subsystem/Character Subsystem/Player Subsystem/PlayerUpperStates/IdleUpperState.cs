using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;

    // Start is called before the first frame update

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(PLAYER_UPPER_STATE.IDLE, this);
    }

    public PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.IDLE; }

    public void StartState()
    {

    }
    public void UpdateState()
    {

    }
    public void EndState(PLAYER_UPPER_STATE _)
    {

    }
    public void Disable() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.DISABLED); }
    public void Move() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.RUNNING); }
    public void OnAir() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING); }
    public void LookUp(bool lookUp) { if(lookUp) playerController.ChangeUpperState(PLAYER_UPPER_STATE.LOOKING_UP);}
    public void Aim(Vector3 aim)
    {
        if (aim != Vector3.zero && playerController.StandingGround != null)
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.AIMING);
    }
    public void Attack() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.ATTACKING); }


    /***** Inavailable State Change *****/
    public void Jump() { return; }
    public void Stop() { return; }
    public void OnGround() { return; }
    public void Enable() { return; }
}
