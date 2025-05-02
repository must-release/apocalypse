using UnityEngine;
using CharacterEnums;

public class HeroineAttackingUpperState : PlayerUpperStateBase<HeroineUpperState>
{
    /****** Public Members ******/

    public override HeroineUpperState StateType => HeroineUpperState.Attacking;

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

        var nextState = PlayerInfo.StandingGround == null ? HeroineUpperState.Jumping : HeroineUpperState.Idle;
        StateController.ChangeState(nextState);
    }

    public override void OnExit(HeroineUpperState _)
    {
        
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

    /****** Private Members ******/

    private float _attackingTime = 0;
}
