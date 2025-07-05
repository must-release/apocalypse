using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class HeroineLookingUpUpperState : IHeroineUpperState
{
    /****** Public Members ******/

    public override HeroineUpperStateType StateType => HeroineUpperStateType.LookingUp;

    public override void InitializeState(IStateController<HeroineUpperStateType> stateController,
                                        IObjectInteractor objectInteractor,
                                        IMotionController playerMotion,
                                        ICharacterInfo playerInfo,
                                        Animator stateAnimator,
                                        PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);

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

    public override void OnExit(HeroineUpperStateType nextState)
    {

    }

    public override void LookUp(bool lookUp) 
    { 
        if(lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroineUpperStateType.Disabled : HeroineUpperStateType.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        if (HorizontalDirection.None == horizontalInput)
        {
            if (_RunningLookingUpStateHash == stateInfo.shortNameHash)
            {
                StateAnimator.Play(_IdleLookingUpStateHash, 0, LowerBodyStateInfo.AnimationNormalizedTime);
                StateAnimator.Update(0.0f);
            }
        }
        else
        {
            if (_IdleLookingUpStateHash == stateInfo.shortNameHash)
            {
                StateAnimator.Play(_RunningLookingUpStateHash, 0, LowerBodyStateInfo.AnimationNormalizedTime);
                StateAnimator.Update(0.0f);
            }
        }

    }

    public override void Disable()
    {
        StateController.ChangeState(HeroineUpperStateType.Disabled);
    }


    /****** Private Members ******/

    private readonly int _IdleLookingUpStateHash    = AnimatorState.Heroine.GetHash(HeroineUpperStateType.Idle, "LookingUp");
    private readonly int _RunningLookingUpStateHash = AnimatorState.Heroine.GetHash(HeroineUpperStateType.Running, "LookingUp");
}
