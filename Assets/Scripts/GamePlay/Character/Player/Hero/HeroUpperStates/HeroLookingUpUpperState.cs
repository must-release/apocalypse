using NUnit.Framework;
using UnityEngine;

public class HeroLookingUpUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.LookingUp;

    public override void InitializeState(IStateController<HeroUpperState> stateController, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, playerMotion, playerInfo, stateAnimator, playerWeapon);
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

    public override void OnExit(HeroUpperState nextState)
    {

    }

    public override void LookUp(bool lookUp)
    {
        if (lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroUpperState.Jumping : HeroUpperState.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Attack()
    {
        base.Attack();

        StateController.ChangeState(HeroUpperState.TopAttacking);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroUpperState.Disabled);
    }


    /****** Private Members ******/

    private readonly int _LookingUpStateHash = AnimatorState.Hero.GetHash(HeroUpperState.LookingUp);
}
