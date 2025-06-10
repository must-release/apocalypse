using UnityEngine;

public class HeroineDisabledUpperState : HeroineUpperStateBase
{
    public override HeroineUpperState StateType => HeroineUpperState.Disabled;

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.Heroine.GetHash(StateType));
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroineUpperState _)
    {
    }

    public override void Enable()
    {
        StateController.ChangeState(HeroineUpperState.Idle);
    }
}
