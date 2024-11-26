using UnityEngine;
using CharacterEums;

public class AimAttackingUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private float attackCoolTime;
    private Vector3 aimingPosition;
    private int direction;

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(PLAYER_UPPER_STATE.AIM_ATTACKING, this);
    }

    public PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.AIM_ATTACKING; }

    public void StartState()
    {
        attackCoolTime = playerController.CurrentPlayer.Attack();
    }

    public void UpdateState()
    {
        // Wait for attacking animation
        attackCoolTime -= Time.deltaTime;
        if (attackCoolTime < 0)
        {
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.AIMING);
        }

        // Set player's looking direction
        SetDirection();

        // Rotate upper body toward the aiming position
        playerController.CurrentPlayer.RotateUpperBody(aimingPosition);
    }

    public void EndState(PLAYER_UPPER_STATE nextState)
    {
        // Recover player's upper body rotation when not aiming or attacking
        if (nextState != PLAYER_UPPER_STATE.AIMING && nextState != PLAYER_UPPER_STATE.AIM_ATTACKING)
            playerController.CurrentPlayer.RotateUpperBody(0);
    }

    public void Disable() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.DISABLED); }

    public void Attack() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.AIM_ATTACKING); }

    public void Aim(Vector3 aim)
    {
        if(aim == Vector3.zero)
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE); 
        else
            aimingPosition = aim;
    }

    // Set player's looking direction
    private void SetDirection()
    {
        direction = aimingPosition.x > playerTransform.position.x ? 1 : -1;
        playerTransform.localScale = new Vector3(direction * Mathf.Abs(playerTransform.localScale.x),
            playerTransform.localScale.y, playerTransform.localScale.z);
    }

    /***** Inavailable State Change *****/
    public void LookUp(bool lookUp) { return;}
    public void Stop() { return; }
    public void Jump() { return; }
    public void OnAir() { return; }
    public void Move() { return; }
    public void OnGround() { return; }
    public void Enable() {return;}
}
