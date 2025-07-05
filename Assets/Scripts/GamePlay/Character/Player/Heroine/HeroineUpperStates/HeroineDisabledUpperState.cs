using UnityEngine;

public class HeroineDisabledUpperState : IHeroineUpperState
{
    public override UpperStateType StateType => HeroineUpperStateType.Disabled;

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.GetHash(PlayerType.Heroine, StateType));
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(UpperStateType _)
    {

    }

    public override void Enable()
    {
        StateController.ChangeState(HeroineUpperStateType.Idle);
    }
}
