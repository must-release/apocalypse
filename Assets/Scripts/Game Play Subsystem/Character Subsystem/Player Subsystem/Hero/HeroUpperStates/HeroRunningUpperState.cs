using UnityEngine;

public class HeroRunningUpperState : PlayerUpperStateBase<HeroUpperState>
{
    public override HeroUpperState StateType =>  HeroUpperState.Running;

    public override void OnEnter()
    {
        //OwnerController.CurrentAnimator.PlayRunning();
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroUpperState _)
    {

    }

    public override void Jump() 
    { 
        StateController.ChangeState(HeroUpperState.Jumping);
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
    
    public override void Stop() 
    { 
        StateController.ChangeState(HeroUpperState.Idle); 
    }
}
