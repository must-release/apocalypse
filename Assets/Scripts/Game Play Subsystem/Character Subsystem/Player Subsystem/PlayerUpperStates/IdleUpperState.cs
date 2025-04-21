using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleUpperState : PlayerUpperStateBase
{
    protected override void StartUpperState()
    {
        playerController.RegisterUpperState(PlayerUpperState.Idle, this);
    }

    public override PlayerUpperState GetStateType() { return PlayerUpperState.Idle; }

    public override void OnEnter()
    {
        playerController.UpperAnimator.PlayRunning();
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(PlayerUpperState _)
    {

    }

    public override void Move() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.Running); 
    }

    public override void OnAir() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.Jumping); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if ( lookUp ) 
            playerController.ChangeUpperState(PlayerUpperState.LookingUp);
    }

    public override void Attack() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.ATTACKING); 
    }


    /***** Inavailable State Change *****/
    public override void Jump() { return; }
    public override void Stop() { return; }
    public override void OnGround() { return; }
    public override void Enable() { return; }
}
