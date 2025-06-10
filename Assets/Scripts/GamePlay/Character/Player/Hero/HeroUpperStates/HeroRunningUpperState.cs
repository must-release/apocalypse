using NUnit.Framework;
using UnityEngine;

public class HeroRunningUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.Running;

    public override void InitializeState(IStateController<HeroUpperState> stateController, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _RunningStateHash), "Hero animator does not have running upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_RunningStateHash, 0, LowerBodyStateInfo.AnimationNormalizedTime);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroUpperState _)
    {

    }

    public override void LookUp(bool lookUp)
    {
        if (false == lookUp) return;

        StateController.ChangeState(HeroUpperState.LookingUp);
    }

    public override void Stop()
    {
        StateController.ChangeState(HeroUpperState.Idle);
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroUpperState.Attacking);
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

    private readonly int _RunningStateHash = AnimatorState.Hero.GetHash(HeroUpperState.Running);
}
