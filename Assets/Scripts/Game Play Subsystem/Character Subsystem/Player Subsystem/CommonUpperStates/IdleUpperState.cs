using CharacterEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleUpperState : PlayerUpperStateBase
{
    public override CommonPlayerUpperState GetStateType() { return CommonPlayerUpperState.Idle; }

    public override void OnEnter()
    {
        OwnerController.CurrentAnimator.PlayIdle();
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(CommonPlayerUpperState _)
    {

    }

    public override void Move() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Running); 
    }

    public override void OnAir() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Jumping); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if ( lookUp ) 
            OwnerController.ChangeUpperState(CommonPlayerUpperState.LookingUp);
    }

    public override void Attack() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Attacking); 
    }


    /***** Inavailable State Change *****/
    public override void Jump() { return; }
    public override void Stop() { return; }
    public override void OnGround() { return; }
    public override void Enable() { return; }
}
