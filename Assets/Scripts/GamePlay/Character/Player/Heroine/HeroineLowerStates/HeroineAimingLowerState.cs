using UnityEngine;

public class HeroineAimingLowerState : HeroineLowerStateBase
{
    public override HeroineLowerState   StateType               => HeroineLowerState.Aiming;
    public override bool                ShouldDisableUpperBody  => true;

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector3.zero);
        StateAnimator.Play(AnimatorState.HeroineLower.Aiming);

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

    public override void OnExit(HeroineLowerState nextState)
    {
        PlayerWeapon.Aim(false);

        if (HeroineLowerState.AimAttacking != nextState)
            PlayerWeapon.RotateWeaponPivot(0);
    }

    public override void Aim(Vector3 aim) 
    { 
        if(Vector3.zero == aim) 
            StateController.ChangeState(HeroineLowerState.Idle);

        _aimingPosition = aim;
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerState.AimAttacking);
    }

    public override void OnAir() 
    {
        StateController.ChangeState(HeroineLowerState.Jumping); 
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerState.Damaged); 
    }



    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineLower.Aiming;


    /****** Private Members ******/

    private Vector3 _aimingPosition = Vector3.zero;

    private void SetDirection()
    {
        var direction = PlayerInfo.CurrentPosition.x < _aimingPosition.x ? FacingDirection.Right : FacingDirection.Left;
        if (direction != PlayerInfo.CurrentFacingDirection)
            PlayerMotion.SetFacingDirection(direction);
    }
}
