using NUnit.Framework;
using UnityEngine;

public class HeroineIdleUpperState : IHeroineUpperState
{
    public override HeroineUpperStateType StateType => HeroineUpperStateType.Idle;

    public override void InitializeState(IStateController<HeroineUpperStateType> stateController,
                                        IObjectInteractor objectInteractor,
                                        IMotionController playerMotion,
                                        ICharacterInfo playerInfo,
                                        Animator stateAnimator,
                                        PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);

        Assert.IsTrue(StateAnimator.HasState(0, _IdleStateHash), "Animator does not have idle upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_IdleStateHash, 0, LowerBodyStateInfo.AnimationNormalizedTime);
        StateAnimator.Update(0.0f);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroineUpperStateType _)
    {

    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        if (HorizontalDirection.None == horizontalInput)
            return;

        StateController.ChangeState(HeroineUpperStateType.Running); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if (false == lookUp) return;

        StateController.ChangeState(HeroineUpperStateType.LookingUp);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroineUpperStateType.Disabled);
    }


    /****** Private Members ******/

    private static readonly int _IdleStateHash = AnimatorState.Heroine.GetHash(HeroineUpperStateType.Idle);
}
