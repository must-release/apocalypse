using UnityEngine;
using CharacterEums;

public class RunningUpperState : PlayerUpperStateBase
{
    public override CommonPlayerUpperState GetStateType() { return CommonPlayerUpperState.Running; }

    public override void OnEnter()
    {
        OwnerController.CurrentAnimator.PlayRunning();
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(CommonPlayerUpperState _)
    {

    }

    public override void Jump() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Jumping); 
    }

    public override void OnAir() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Jumping); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if( lookUp ) 
            OwnerController.ChangeUpperState(CommonPlayerUpperState.LookingUp);
    }

    public override void Attack() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Attacking); 
    }
    
    public override void Stop() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Idle); 
    }


    /***** Inavailable State Change *****/
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
