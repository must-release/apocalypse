using UnityEngine;
using CharacterEums;

public class RunningUpperState : PlayerUpperStateBase
{
    public override PlayerUpperState GetStateType() { return PlayerUpperState.Running; }

    public override void OnEnter()
    {
        OwnerController.UpperAnimator.PlayRunning();
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(PlayerUpperState _)
    {

    }

    public override void Jump() 
    { 
        OwnerController.ChangeUpperState(PlayerUpperState.Jumping); 
    }

    public override void OnAir() 
    { 
        OwnerController.ChangeUpperState(PlayerUpperState.Jumping); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if( lookUp ) 
            OwnerController.ChangeUpperState(PlayerUpperState.LookingUp);
    }

    public override void Attack() 
    { 
        OwnerController.ChangeUpperState(PlayerUpperState.ATTACKING); 
    }
    
    public override void Stop() 
    { 
        OwnerController.ChangeUpperState(PlayerUpperState.Idle); 
    }


    /***** Inavailable State Change *****/
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
