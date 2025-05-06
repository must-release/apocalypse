using UnityEngine;

public class HeroAimingUpperState : PlayerUpperStateBase<HeroUpperState>
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.Aiming;

    public override void OnEnter()
    {
        // Enable fixed update
        _fixedUpdateFlag = true;
    }

    public override void OnUpdate()
    {
        // Set player's looking direction
        SetDirection();

        // Rotate upper body toward the aiming position
        //OwnerController.CurrentAvatar.RotateUpperBody(_aimingPosition);
    }

    private void FixedUpdate() 
    {   
        if(false == _fixedUpdateFlag) return;

        //OwnerController.CurrentAvatar.Aim(true);
    }

    public override void OnExit(HeroUpperState nextState)
    {
        // Turn off player's aiming state
        //OwnerController.CurrentAvatar.Aim(false);

        // Recover player's upper body rotation when not attacking
        // if(nextState != HeroUpperState.AimAttacking)
        //     OwnerController.CurrentAvatar.RotateUpperBody(0);

        // Disable fixed update
        _fixedUpdateFlag = false;
    }

    public override void OnAir() 
    { 
        StateController.ChangeState(HeroUpperState.Jumping); 
    }

    public override void Attack() 
    {
        StateController.ChangeState(HeroUpperState.AimAttacking); 
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim)
            StateController.ChangeState(HeroUpperState.Idle); 
        else
            _aimingPosition = aim;
    }

    /****** Private Members ******/

    private Vector3 _aimingPosition     = Vector3.zero;
    private bool    _fixedUpdateFlag    = false;

    private void SetDirection()
    {
        var direction = PlayerInfo.CurrentPosition.x < _aimingPosition.x ? FacingDirection.Right : FacingDirection.Left;
        if (direction != PlayerInfo.FacingDirection)
        {
            PlayerMotion.SetFacingDirection(direction);
        }
    }
}
