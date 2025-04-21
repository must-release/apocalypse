using UnityEngine;
using CharacterEums;

public class AimAttackingUpperState : PlayerUpperStateBase
{
    private float attackCoolTime;
    private Vector3 aimingPosition;
    private int direction;

    protected override void StartUpperState()
    {
        playerController.RegisterUpperState(PlayerUpperState.AimAttacking, this);
    }

    public override PlayerUpperState GetStateType() { return PlayerUpperState.AimAttacking; }

    public override void OnEnter()
    {
        attackCoolTime = playerController.CurrentAvatar.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        attackCoolTime -= Time.deltaTime;
        if ( 0 < attackCoolTime )
            playerController.ChangeUpperState(PlayerUpperState.Aiming);

        // Set player's looking direction
        SetDirection();

        // Rotate upper body toward the aiming position
        playerController.CurrentAvatar.RotateUpperBody(aimingPosition);
    }

    public override void OnExit(PlayerUpperState nextState)
    {
        // Recover player's upper body rotation when not aiming or attacking
        if (PlayerUpperState.Aiming == nextState || PlayerUpperState.AimAttacking == nextState)
            return;

        playerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void Disable() { playerController.ChangeUpperState(PlayerUpperState.Disabled); }

    public override void Attack() { playerController.ChangeUpperState(PlayerUpperState.AimAttacking); }

    public override void Aim(Vector3 aim)
    {
        if(Vector3.zero == aim )
            playerController.ChangeUpperState(PlayerUpperState.Idle); 
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
