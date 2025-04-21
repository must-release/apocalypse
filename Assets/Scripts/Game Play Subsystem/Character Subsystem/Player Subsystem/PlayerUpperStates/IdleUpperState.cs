using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleUpperState : PlayerUpperStateBase
{
    public override PlayerUpperState GetStateType() { return PlayerUpperState.Idle; }

    public override void OnEnter()
    {
        OwnerController.UpperAnimator.PlayIdle();
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(PlayerUpperState _)
    {

    }

    public override void Move() 
    { 
        OwnerController.ChangeUpperState(PlayerUpperState.Running); 
    }

    public override void OnAir() 
    { 
        OwnerController.ChangeUpperState(PlayerUpperState.Jumping); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if ( lookUp ) 
            OwnerController.ChangeUpperState(PlayerUpperState.LookingUp);
    }

    public override void Attack() 
    { 
        OwnerController.ChangeUpperState(PlayerUpperState.ATTACKING); 
    }


    /***** Inavailable State Change *****/
    public override void Jump() { return; }
    public override void Stop() { return; }
    public override void OnGround() { return; }
    public override void Enable() { return; }
}
