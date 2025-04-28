using UnityEngine;
using CharacterEnums;

public class AimAttackingUpperState : PlayerUpperStateBase
{
    private float attackCoolTime;
    private Vector3 aimingPosition;
    private int direction;

    public override CommonPlayerUpperState GetStateType() { return CommonPlayerUpperState.AimAttacking; }

    public override void OnEnter()
    {
        attackCoolTime = OwnerController.CurrentAvatar.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        attackCoolTime -= Time.deltaTime;
        if ( 0 < attackCoolTime )
            OwnerController.ChangeUpperState(CommonPlayerUpperState.Aiming);

        // Set player's looking direction
        SetDirection();

        // Rotate upper body toward the aiming position
        OwnerController.CurrentAvatar.RotateUpperBody(aimingPosition);
    }

    public override void OnExit(CommonPlayerUpperState nextState)
    {
        // Recover player's upper body rotation when not aiming or attacking
        if (CommonPlayerUpperState.Aiming == nextState || CommonPlayerUpperState.AimAttacking == nextState)
            return;

        OwnerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void Disable() { OwnerController.ChangeUpperState(CommonPlayerUpperState.Disabled); }

    public override void Attack() { OwnerController.ChangeUpperState(CommonPlayerUpperState.AimAttacking); }

    public override void Aim(Vector3 aim)
    {
        if(Vector3.zero == aim )
            OwnerController.ChangeUpperState(CommonPlayerUpperState.Idle); 
        else
            aimingPosition = aim;
    }

    // Set player's looking direction
    private void SetDirection()
    {
        direction = aimingPosition.x > OwnerTransform.position.x ? 1 : -1;
        OwnerTransform.localScale = new Vector3(direction * Mathf.Abs(OwnerTransform.localScale.x),
            OwnerTransform.localScale.y, OwnerTransform.localScale.z);
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
