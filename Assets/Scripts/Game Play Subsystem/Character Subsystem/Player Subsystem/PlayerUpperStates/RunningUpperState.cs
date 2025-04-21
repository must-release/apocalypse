using UnityEngine;
using CharacterEums;

public class RunningUpperState : PlayerUpperStateBase
{
    protected override void StartUpperState()
    {
        playerController.RegisterUpperState(PlayerUpperState.Running, this);
    }

    public override PlayerUpperState GetStateType() { return PlayerUpperState.Running; }

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

    public override void Jump() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.Jumping); 
    }

    public override void OnAir() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.Jumping); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if( lookUp ) 
            playerController.ChangeUpperState(PlayerUpperState.LookingUp);
    }

    public override void Attack() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.ATTACKING); 
    }
    
    public override void Stop() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.Idle); 
    }


    /***** Inavailable State Change *****/
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
