using UnityEngine;

public class HeroineRunningUpperState : PlayerUpperStateBase<HeroineUpperState>
{
    public override HeroineUpperState StateType =>  HeroineUpperState.Running;

    public override void OnEnter()
    {
        //OwnerController.CurrentAnimator.PlayRunning();
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroineUpperState _)
    {

    }

    public override void Jump() 
    { 
        StateController.ChangeState(HeroineUpperState.Jumping);
    }

    public override void OnAir() 
    { 
        StateController.ChangeState(HeroineUpperState.Jumping);
    }

    public override void LookUp(bool lookUp) 
    { 
        if (false == lookUp) return;

        StateController.ChangeState(HeroineUpperState.LookingUp);
    }

    public override void Attack() 
    { 
        StateController.ChangeState(HeroineUpperState.Attacking); 
    }
    
    public override void Stop() 
    { 
        StateController.ChangeState(HeroineUpperState.Idle); 
    }
}
