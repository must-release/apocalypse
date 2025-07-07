using UnityEngine;


public class HeroineAimingLowerState : CommonAimingLowerState
{
    /******* Public Members ******/

    public override bool ShouldDisableUpperBody => true;

    public override void OnEnter()
    {
        base.OnEnter(); // For playing animation and setting velocity

        _aimingPosition = Vector3.zero;
    }

    public override void OnUpdate()
    {
        SetDirection();

        PlayerWeapon.RotateWeaponPivot(_aimingPosition);
    }

    public override void OnFixedUpdate()
    {
        PlayerWeapon.Aim(true);
    }

    public override void OnExit(LowerStateType nextState)
    {
        PlayerWeapon.Aim(false);

        if (HeroineLowerStateType.AimAttacking != nextState)
            PlayerWeapon.SetWeaponPivotRotation(0);
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim)
            StateController.ChangeState(LowerStateType.Idle);

        _aimingPosition = aim;
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerStateType.AimAttacking);
    }


    /******* Private Members ******/

    private Vector3 _aimingPosition;
    private void SetDirection()
    {
        var direction = PlayerInfo.CurrentPosition.x < _aimingPosition.x ? FacingDirection.Right : FacingDirection.Left;
        if (direction != PlayerInfo.CurrentFacingDirection)
            PlayerMotion.SetFacingDirection(direction);
    }
}
