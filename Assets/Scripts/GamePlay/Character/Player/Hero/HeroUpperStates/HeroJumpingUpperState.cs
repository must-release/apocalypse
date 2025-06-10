using NUnit.Framework;
using UnityEngine;

public class HeroJumpingUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.Jumping;

    public override void InitializeState(IStateController<HeroUpperState> stateController, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _JumpingStateHash), $"Hero animator does not have jumping upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_JumpingStateHash);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroUpperState _)
    {
    }


    /****** Private Members ******/

    private readonly int _JumpingStateHash = AnimatorState.Hero.GetHash(HeroUpperState.Jumping);
}
