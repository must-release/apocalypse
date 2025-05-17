using UnityEngine;

public class HeroDisabledUpperState : PlayerUpperStateBase<HeroUpperState>
{
    public override HeroUpperState StateType => HeroUpperState.Disabled;

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroUpperState _)
    {

    }

    public override void Enable()
    {
        var nextState = PlayerInfo.StandingGround == null ? HeroUpperState.Jumping : HeroUpperState.Idle;
        StateController.ChangeState(nextState);
    }
}
