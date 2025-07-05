using NUnit.Framework;
using UnityEngine;

public class HeroDisabledUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperStateType StateType => HeroUpperStateType.Disabled;

    public override void InitializeState(IStateController<HeroUpperStateType> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _DisableStateHash), $"Hero animator does not have disabled upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_DisableStateHash);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroUpperStateType _)
    {
    }

    public override void Enable()
    {
        StateController.ChangeState(HeroUpperStateType.Idle);
    }


    /****** Private Members ******/

    private readonly int _DisableStateHash = AnimatorState.Hero.GetHash(HeroUpperStateType.Disabled);
}
