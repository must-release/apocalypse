using UnityEngine;

public class HeroineAimingLowerState : HeroineLowerState
{
    public override HeroineLowerStateType   StateType               => HeroineLowerStateType.Aiming;
    public override bool                ShouldDisableUpperBody  => true;

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector3.zero);
        StateAnimator.Play(AnimatorState.Heroine.GetHash(StateType));

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

    public override void OnExit(HeroineLowerStateType nextState)
    {
        PlayerWeapon.Aim(false);

        if (HeroineLowerStateType.AimAttacking != nextState)
            PlayerWeapon.SetWeaponPivotRotation(0);
    }

    public override void Aim(Vector3 aim) 
    { 
        if(Vector3.zero == aim) 
            StateController.ChangeState(HeroineLowerStateType.Idle);

        _aimingPosition = aim;
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerStateType.AimAttacking);
    }

    public override void OnAir() 
    {
        StateController.ChangeState(HeroineLowerStateType.Jumping); 
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerStateType.Damaged); 
    }


    /****** Private Members ******/

    private Vector3 _aimingPosition = Vector3.zero;

    private void SetDirection()
    {
        var direction = PlayerInfo.CurrentPosition.x < _aimingPosition.x ? FacingDirection.Right : FacingDirection.Left;
        if (direction != PlayerInfo.CurrentFacingDirection)
            PlayerMotion.SetFacingDirection(direction);
    }
}
