using UnityEngine;

public class HeroineDisabledUpperState : PlayerUpperStateBase<HeroineUpperState>
{
    public override HeroineUpperState StateType => HeroineUpperState.Disabled;

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroineUpperState _)
    {

    }

    public override void Enable()
    {
        var nextState = PlayerInfo.StandingGround == null ? HeroineUpperState.Jumping : HeroineUpperState.Idle;
        StateController.ChangeState(nextState);
    }
}
