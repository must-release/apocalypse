using UnityEngine;
using CharacterEums;

public class AimingUpperState : PlayerUpperStateBase
{
    private Vector3 aimingPosition;
    private int direction;
    private bool fixedUpdateFlag;
    

    protected override void StartUpperState()
    {
        playerController.RegisterUpperState(PlayerUpperState.Aiming, this);
    }

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
        playerController.CurrentAvatar.RotateUpperBody(aimingPosition);
    }

    private void FixedUpdate() 
    {   
        // Turn on player's aiming state
        if(fixedUpdateFlag) 
            playerController.CurrentAvatar.Aim(true);
    }

    public override void OnExit(PlayerUpperState nextState)
    {
        // Turn off player's aiming state
        playerController.CurrentAvatar.Aim(false);

        // Recover player's upper body rotation when not attacking
        if(nextState != PlayerUpperState.AimAttacking)
            playerController.CurrentAvatar.RotateUpperBody(0);

        // Disable fixed update
        fixedUpdateFlag = false;
    }

    public override void OnAir() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.Jumping); 
    }

    public override void Attack() 
    {
        playerController.ChangeUpperState(PlayerUpperState.AimAttacking); 
    }

    public override void Aim(Vector3 aim)
    {
        if ( Vector3.zero == aim )
            playerController.ChangeUpperState(PlayerUpperState.Idle); 
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
    public override void Jump() { return; }
    public override void Stop() { return; }
    public override void LookUp(bool lookUp) { return; }
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
