using UnityEngine;

public class HeroJumpingUpperState : PlayerUpperStateBase<HeroUpperState>
{
    public override HeroUpperState StateType => HeroUpperState.Jumping;

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroUpperState _)
    {

    }

    public override void OnGround() 
    { 
        StateController.ChangeState(HeroUpperState.Idle); 
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
