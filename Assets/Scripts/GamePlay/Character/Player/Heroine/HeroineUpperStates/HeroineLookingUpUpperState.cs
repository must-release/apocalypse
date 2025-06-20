using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class HeroineLookingUpUpperState : HeroineUpperStateBase
{
    /****** Public Members ******/

    public override HeroineUpperState StateType => HeroineUpperState.LookingUp;

    public override void InitializeState(IStateController<HeroineUpperState> stateController
                                         , IMotionController playerMotion
                                         , ICharacterInfo playerInfo
                                         , Animator stateAnimator
                                         , PlayerWeaponBase playerWeapon
    )
    {
        base.InitializeState(stateController, playerMotion, playerInfo, stateAnimator, playerWeapon);

        Assert.IsTrue(StateAnimator.HasState(0, _IdleLookingUpStateHash), "Animator does not have idle looking up state.");
        Assert.IsTrue(StateAnimator.HasState(0, _RunningLookingUpStateHash), "Animator does not have running looking up state.");
    }

    public override void OnEnter()
    {
        var nextStateHash = PlayerInfo.IsMoving ? _RunningLookingUpStateHash : _IdleLookingUpStateHash;
        StateAnimator.Play(nextStateHash, 0, LowerBodyStateInfo.AnimationNormalizedTime);
        StateAnimator.Update(0.0f);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(HeroineUpperState nextState)
    {

    }

    public override void LookUp(bool lookUp) 
    { 
        if(lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroineUpperState.Disabled : HeroineUpperState.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Move()
    {
        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        if (_IdleLookingUpStateHash == stateInfo.shortNameHash)
        {
            StateAnimator.Play(_RunningLookingUpStateHash, 0, LowerBodyStateInfo.AnimationNormalizedTime);
            StateAnimator.Update(0.0f);
        }
    }

    public override void Stop()
    {
        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        if (_RunningLookingUpStateHash == stateInfo.shortNameHash)
        {
            StateAnimator.Play(_IdleLookingUpStateHash, 0, LowerBodyStateInfo.AnimationNormalizedTime);
            StateAnimator.Update(0.0f);
        }
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroineUpperState.Disabled);
    }


    /****** Private Members ******/

    private readonly int _IdleLookingUpStateHash    = AnimatorState.Heroine.GetHash(HeroineUpperState.Idle, "LookingUp");
    private readonly int _RunningLookingUpStateHash = AnimatorState.Heroine.GetHash(HeroineUpperState.Running, "LookingUp");
}
