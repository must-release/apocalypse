using NUnit.Framework;
using UnityEngine;

public class HeroAimingLowerState : HeroLowerStateBase
{
    public override HeroLowerStateType StateType    => HeroLowerStateType.Aiming;
    public override bool ShouldDisableUpperBody => false;

    public override void InitializeState(IStateController<HeroLowerStateType> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);

        Assert.IsTrue(StateAnimator.HasState(0, _AimingStateHash), "Hero animator does not have aiming lower state.");
    }

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector3.zero);
        StateAnimator.Play(_AimingStateHash);
        StateAnimator.Update(0.0f);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(HeroLowerStateType nextState)
    {

    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim)
            StateController.ChangeState(HeroLowerStateType.Idle);
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroLowerStateType.AimAttacking);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroLowerStateType.Jumping);
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroLowerStateType.Damaged);
    }


    /****** Private Members ******/
    
    private readonly int _AimingStateHash = AnimatorState.Hero.GetHash(HeroLowerStateType.Aiming);
}
