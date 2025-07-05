using NUnit.Framework;
using UnityEngine;

public class HeroJumpingUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperStateType StateType => HeroUpperStateType.Jumping;

    public override void InitializeState(IStateController<HeroUpperStateType> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _JumpingStateHash), $"Hero animator does not have jumping upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_JumpingStateHash);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroUpperStateType _)
    {
    }


    /****** Private Members ******/

    private readonly int _JumpingStateHash = AnimatorState.Hero.GetHash(HeroUpperStateType.Jumping);
}
