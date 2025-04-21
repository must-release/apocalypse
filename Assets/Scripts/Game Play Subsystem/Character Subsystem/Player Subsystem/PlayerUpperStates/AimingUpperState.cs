using UnityEngine;
using CharacterEums;

public class AimingUpperState : PlayerUpperStateBase
{
    private Vector3 aimingPosition;
    private int direction;
    private bool fixedUpdateFlag;

    public override PlayerUpperState GetStateType() { return PlayerUpperState.Aiming; }

    public override void OnEnter()
    {
        // Enable fixed update
        fixedUpdateFlag = true;
    }
    public override void OnUpdate()
    {
        // Set player's looking direction
        SetDirection();

        // Rotate upper body toward the aiming position
        OwnerController.CurrentAvatar.RotateUpperBody(aimingPosition);
    }

    private void FixedUpdate() 
    {   
        // Turn on player's aiming state
        if(fixedUpdateFlag) 
            OwnerController.CurrentAvatar.Aim(true);
    }

    public override void OnExit(PlayerUpperState nextState)
    {
        // Turn off player's aiming state
        OwnerController.CurrentAvatar.Aim(false);

        // Recover player's upper body rotation when not attacking
        if(nextState != PlayerUpperState.AimAttacking)
            OwnerController.CurrentAvatar.RotateUpperBody(0);

        // Disable fixed update
        fixedUpdateFlag = false;
    }

    public override void OnAir() 
    { 
        OwnerController.ChangeUpperState(PlayerUpperState.Jumping); 
    }

    public override void Attack() 
    {
        OwnerController.ChangeUpperState(PlayerUpperState.AimAttacking); 
    }

    public override void Aim(Vector3 aim)
    {
        if ( Vector3.zero == aim )
            OwnerController.ChangeUpperState(PlayerUpperState.Idle); 
        else
            aimingPosition = aim;
    }

    private void SetDirection()
    {
        direction = aimingPosition.x > OwnerTransform.position.x ? 1 : -1;
        OwnerTransform.localScale = new Vector3(direction * Mathf.Abs(OwnerTransform.localScale.x),
            OwnerTransform.localScale.y, OwnerTransform.localScale.z);
    }

    /***** Inavailable State Change *****/
    public override void Jump() { return; }
    public override void Stop() { return; }
    public override void LookUp(bool lookUp) { return; }
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
