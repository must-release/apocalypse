using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class HeroAimingUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.Aiming;

    public override void InitializeState(IStateController<HeroUpperState> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _AimingStateHash), "Hero animator does not have aiming upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_AimingStateHash);
        StateAnimator.Update(0.0f);

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

    public override void OnExit(HeroUpperState nextState)
    {
        PlayerWeapon.Aim(false);

        if (HeroUpperState.AimAttacking != nextState)
            PlayerWeapon.SetWeaponPivotRotation(0);
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim)
            StateController.ChangeState(HeroUpperState.Idle);

        _aimingPosition = aim;
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroUpperState.AimAttacking);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroUpperState.Jumping);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroUpperState.Disabled);
    }



    /****** Private Members ******/

    private readonly int _AimingStateHash = AnimatorState.Hero.GetHash(HeroUpperState.Attacking);

    private Vector3 _aimingPosition = Vector3.zero;

    private void SetDirection()
    {
        var direction = PlayerInfo.CurrentPosition.x < _aimingPosition.x ? FacingDirection.Right : FacingDirection.Left;
        if (direction != PlayerInfo.CurrentFacingDirection)
            PlayerMotion.SetFacingDirection(direction);
    }
}
