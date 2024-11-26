using UnityEngine;
using CharacterEums;

public class AimingUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Vector3 aimingPosition;
    private int direction;
    private bool fixedUpdateFlag;
    

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(PLAYER_UPPER_STATE.AIMING, this);
    }

    public PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.AIMING; }

    public void StartState()
    {
        // Enable fixed update
        fixedUpdateFlag = true;
    }
    public void UpdateState()
    {
        // Set player's looking direction
        SetDirection();

        // Rotate upper body toward the aiming position
        playerController.CurrentPlayer.RotateUpperBody(aimingPosition);
    }

    private void FixedUpdate() 
    {   
        // Turn on player's aiming state
        if(fixedUpdateFlag) playerController.CurrentPlayer.Aim(true);
    }

    public void EndState(PLAYER_UPPER_STATE nextState)
    {
        // Turn off player's aiming state
        playerController.CurrentPlayer.Aim(false);

        // Recover player's upper body rotation when not attacking
        if(nextState != PLAYER_UPPER_STATE.AIM_ATTACKING)
            playerController.CurrentPlayer.RotateUpperBody(0);

        // Disable fixed update
        fixedUpdateFlag = false;
    }

    public void Disable() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.DISABLED); }

    public void OnAir() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING); }

    public void Attack() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.AIM_ATTACKING); }

    public void Aim(Vector3 aim)
    {
        if(aim == Vector3.zero)
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE); 
        else
            aimingPosition = aim;
    }

    private void SetDirection()
    {
        direction = aimingPosition.x > playerTransform.position.x ? 1 : -1;
        playerTransform.localScale = new Vector3(direction * Mathf.Abs(playerTransform.localScale.x),
            playerTransform.localScale.y, playerTransform.localScale.z);
    }

    /***** Inavailable State Change *****/
    public void Jump() { return; }
    public void Stop() { return; }
    public void LookUp(bool lookUp) { return; }
    public void Move() { return; }
    public void OnGround() { return; }
    public void Enable() {return;}
}
