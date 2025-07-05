using UnityEngine;

public class HeroineRunningUpperState : IHeroineUpperState
{
    public override HeroineUpperStateType StateType => HeroineUpperStateType.Running;

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.Heroine.GetHash(StateType), 0, LowerBodyStateInfo.AnimationNormalizedTime);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroineUpperStateType _)
    {

    }

    public override void LookUp(bool lookUp) 
    { 
        if (false == lookUp) return;

        StateController.ChangeState(HeroineUpperStateType.LookingUp);
    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        if (HorizontalDirection.None != horizontalInput)
            return;

        StateController.ChangeState(HeroineUpperStateType.Idle);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroineUpperStateType.Disabled);
    }
}
