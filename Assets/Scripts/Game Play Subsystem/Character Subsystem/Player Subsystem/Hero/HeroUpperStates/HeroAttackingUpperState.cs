using UnityEngine;

public class HeroiAttackingUpperState : PlayerUpperStateBase<HeroUpperState>
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.Attacking;

    public override void OnEnter()
    {
        // Execute attack and get attacking motion time
        //_attackingTime = OwnerController.CurrentAvatar.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        _attackingTime -= Time.deltaTime;

        if (0 < _attackingTime) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroUpperState.Jumping : HeroUpperState.Idle;
        StateController.ChangeState(nextState);
    }

    public override void OnExit(HeroUpperState _)
    {
        
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

    /****** Private Members ******/

    private float _attackingTime = 0;
}
