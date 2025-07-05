using NUnit.Framework;
using UnityEngine;

public class HeroIdleUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.Idle;

    public override void InitializeState(IStateController<HeroUpperState> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _IdleStateHash), $"Hero animator does not have idle upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_IdleStateHash);
        StateAnimator.Update(0.0f);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroUpperState _)
    {

    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        if (HorizontalDirection.None == horizontalInput)
            return;

        StateController.ChangeState(HeroUpperState.Running);
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroUpperState.Attacking);
    }

    public override void LookUp(bool lookUp)
    {
        if (false == lookUp) return;

        StateController.ChangeState(HeroUpperState.LookingUp);
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim) return;
        StateController.ChangeState(HeroUpperState.Aiming);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroUpperState.Disabled);
    }


    /****** Private Members ******/

    private readonly int _IdleStateHash = AnimatorState.Hero.GetHash(HeroUpperState.Idle);
}
