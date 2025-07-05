using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class HeroDeadLowerState : HeroLowerStateBase
{
    /****** Public Members ******/

    public override HeroLowerState StateType    => HeroLowerState.Dead;
    public override bool ShouldDisableUpperBody => true;

    public override void InitializeState(IStateController<HeroLowerState> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _HeroDeadStateHash), "Hero animator does not have dead lower state.");
    }

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector2.zero);
        StateAnimator.Play(_HeroDeadStateHash);
        StateAnimator.Update(0.0f);

        _isAnimationPlaying = true;
    }

    public override void OnUpdate()
    {
        if (false == _isAnimationPlaying) return;

        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        if (1.0f <= stateInfo.normalizedTime)
        {
            CharacterManager.Instance.ProcessPlayersDeath();
            _isAnimationPlaying = false;
        }
    }

    public override void OnExit(HeroLowerState _)
    {

    }


    /****** Private Members ******/

    private readonly int _HeroDeadStateHash = AnimatorState.Hero.GetHash(HeroLowerState.Dead);

    private bool _isAnimationPlaying = false;
}
