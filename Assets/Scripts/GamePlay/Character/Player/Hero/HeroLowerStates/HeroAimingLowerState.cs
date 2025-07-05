using NUnit.Framework;
using UnityEngine;

public class HeroAimingLowerState : HeroLowerStateBase
{
    public override HeroLowerState StateType    => HeroLowerState.Aiming;
    public override bool ShouldDisableUpperBody => false;

    public override void InitializeState(IStateController<HeroLowerState> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
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

    public override void OnExit(HeroLowerState nextState)
    {

    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim)
            StateController.ChangeState(HeroLowerState.Idle);
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroLowerState.AimAttacking);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroLowerState.Jumping);
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroLowerState.Damaged);
    }


    /****** Private Members ******/
    
    private readonly int _AimingStateHash = AnimatorState.Hero.GetHash(HeroLowerState.Aiming);
}
