using CharacterEnums;
using UnityEngine;

public class HeroineIdleUpperState : PlayerUpperStateBase<HeroineUpperState>
{
    public override HeroineUpperState StateType => HeroineUpperState.Idle;

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroineUpperState _)
    {

    }

    public override void Move() 
    { 
        StateController.ChangeState(HeroineUpperState.Running); 
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
}
