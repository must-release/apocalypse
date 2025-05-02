using UnityEngine;

public class HeroineJumpingUpperState : PlayerUpperStateBase<HeroineUpperState>
{
    public override HeroineUpperState StateType => HeroineUpperState.Jumping;

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroineUpperState _)
    {

    }

    public override void OnGround() 
    { 
        StateController.ChangeState(HeroineUpperState.Idle); 
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
