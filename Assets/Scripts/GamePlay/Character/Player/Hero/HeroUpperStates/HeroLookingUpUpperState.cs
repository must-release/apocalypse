using NUnit.Framework;
using UnityEngine;

public class HeroLookingUpUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperStateType StateType => HeroUpperStateType.LookingUp;

    public override void InitializeState(IStateController<HeroUpperStateType> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _LookingUpStateHash), "Animator does not have looking up state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_LookingUpStateHash, 0, LowerBodyStateInfo.AnimationNormalizedTime);
        StateAnimator.Update(0.0f);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(HeroUpperStateType nextState)
    {

    }

    public override void LookUp(bool lookUp)
    {
        if (lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroUpperStateType.Jumping : HeroUpperStateType.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Attack()
    {
        base.Attack();

        StateController.ChangeState(HeroUpperStateType.TopAttacking);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroUpperStateType.Disabled);
    }


    /****** Private Members ******/

    private readonly int _LookingUpStateHash = AnimatorState.Hero.GetHash(HeroUpperStateType.LookingUp);
}
