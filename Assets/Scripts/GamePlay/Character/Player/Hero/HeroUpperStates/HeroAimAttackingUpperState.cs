using UnityEngine;

public class HeroAimAttackingUpperState : PlayerUpperStateBase<HeroUpperState>
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.AimAttacking;

    public override void OnEnter()
    {
        //_attackCoolTime = OwnerController.CurrentAvatar.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        _attackCoolTime -= Time.deltaTime;

        if ( 0 < _attackCoolTime )
            StateController.ChangeState(HeroUpperState.Aiming);

        // Set player's looking direction
        SetDirection();

        // Rotate upper body toward the aiming position
        //StateController.CurrentAvatar.RotateUpperBody(_aimingPosition);
    }

    public override void OnExit(HeroUpperState nextState)
    {
        // Recover player's upper body rotation when not aiming or attacking
        if (HeroUpperState.Aiming == nextState || HeroUpperState.AimAttacking == nextState)
            return;

        //OwnerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void Disable() { StateController.ChangeState(HeroUpperState.Disabled); }

    public override void Attack() { StateController.ChangeState(HeroUpperState.AimAttacking); }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim)
            StateController.ChangeState(HeroUpperState.Idle); 
        else
            _aimingPosition = aim;
    }


    /****** Private Members ******/

    private float           _attackCoolTime = 0;
    private Vector3         _aimingPosition = Vector3.zero;

    // Set player's looking direction
    private void SetDirection()
    {
        var direction = PlayerInfo.CurrentPosition.x < _aimingPosition.x ? FacingDirection.Right : FacingDirection.Left;
        if (direction != PlayerInfo.CurrentFacingDirection)
        {
            PlayerMotion.SetFacingDirection(direction);
        }
    }
}
