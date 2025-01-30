using UnityEngine;
using CharacterEums;

public class AimAttackingUpperState : PlayerUpperStateBase
{
    private float attackCoolTime;
    private Vector3 aimingPosition;
    private int direction;

    protected override void StartUpperState()
    {
        playerController.AddUpperState(PLAYER_UPPER_STATE.AIM_ATTACKING, this);
    }

    public override PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.AIM_ATTACKING; }

    public override void OnEnter()
    {
        attackCoolTime = playerController.CurrentPlayer.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        attackCoolTime -= Time.deltaTime;
        if ( 0 < attackCoolTime )
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.AIMING);

        // Set player's looking direction
        SetDirection();

        // Rotate upper body toward the aiming position
        playerController.CurrentPlayer.RotateUpperBody(aimingPosition);
    }

    public override void OnExit(PLAYER_UPPER_STATE nextState)
    {
        // Recover player's upper body rotation when not aiming or attacking
        if (PLAYER_UPPER_STATE.AIMING == nextState || PLAYER_UPPER_STATE.AIM_ATTACKING == nextState)
            return;

        playerController.CurrentPlayer.RotateUpperBody(0);
    }

    public override void Disable() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.DISABLED); }

    public override void Attack() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.AIM_ATTACKING); }

    public override void Aim(Vector3 aim)
    {
        if(Vector3.zero == aim )
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
    public override void LookUp(bool lookUp) { return;}
    public override void Stop() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
