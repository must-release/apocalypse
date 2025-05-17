using UnityEngine;

public class HeroIdleUpperState : PlayerUpperStateBase<HeroUpperState>
{
    public override HeroUpperState StateType => HeroUpperState.Idle;

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroUpperState _)
    {

    }

    public override void Move() 
    { 
        StateController.ChangeState(HeroUpperState.Running); 
    }

    public override void OnAir() 
    { 
        StateController.ChangeState(HeroUpperState.Jumping); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if (false == lookUp) return;

        StateController.ChangeState(HeroUpperState.LookingUp);
    }

    public override void Attack() 
    { 
        StateController.ChangeState(HeroUpperState.Attacking);
    }
}
