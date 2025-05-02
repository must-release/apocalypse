using UnityEngine;
using CharacterEnums;

public class HeroineAimAttackingUpperState : PlayerUpperStateBase<HeroineUpperState>
{
    /****** Public Members ******/

    public override HeroineUpperState StateType => HeroineUpperState.AimAttacking;

    public override void OnEnter()
    {
        //_attackCoolTime = OwnerController.CurrentAvatar.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        _attackCoolTime -= Time.deltaTime;

        if ( 0 < _attackCoolTime )
            StateController.ChangeState(HeroineUpperState.Aiming);

        // Set player's looking direction
        SetDirection();

        // Rotate upper body toward the aiming position
        //StateController.CurrentAvatar.RotateUpperBody(_aimingPosition);
    }

    public override void OnExit(HeroineUpperState nextState)
    {
        // Recover player's upper body rotation when not aiming or attacking
        if (HeroineUpperState.Aiming == nextState || HeroineUpperState.AimAttacking == nextState)
            return;

        //OwnerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void Disable() { StateController.ChangeState(HeroineUpperState.Disabled); }

    public override void Attack() { StateController.ChangeState(HeroineUpperState.AimAttacking); }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim)
            StateController.ChangeState(HeroineUpperState.Idle); 
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
        if (direction != PlayerInfo.FacingDirection)
        {
            PlayerMotion.SetFacingDirection(direction);
        }
    }
}
