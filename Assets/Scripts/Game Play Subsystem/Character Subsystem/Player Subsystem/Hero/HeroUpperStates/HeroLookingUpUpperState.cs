using UnityEngine;

public class HeroLookingUpUpperState : PlayerUpperStateBase<HeroUpperState>
{

    public override HeroUpperState StateType => HeroUpperState.LookingUp;

    public override void OnEnter()
    {
        // Rotate upper body by 90 degree
        //OwnerController.CurrentAvatar.RotateUpperBody(90);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(HeroUpperState nextState)
    {
        // Recover upper body rotation when not attacking
        if (HeroUpperState.TopAttacking == nextState)
            return;
        
        //OwnerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void LookUp(bool lookUp) 
    { 
        if(lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroUpperState.Jumping : HeroUpperState.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Attack() 
    { 
        StateController.ChangeState(HeroUpperState.TopAttacking);
    }
}
