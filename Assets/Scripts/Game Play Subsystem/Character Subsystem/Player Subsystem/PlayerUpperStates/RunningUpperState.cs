using UnityEngine;
using CharacterEums;

public class RunningUpperState : PlayerUpperStateBase
{
    protected override void StartUpperState()
    {
        playerController.AddUpperState(PLAYER_UPPER_STATE.RUNNING, this);
    }

    public override PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.RUNNING; }

    public override void OnEnter()
    {
        playerController.UpperAnimator.SetBool("Move", true);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(PLAYER_UPPER_STATE _)
    {

    }

    public override void Jump() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING); 
    }

    public override void OnAir() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if( lookUp ) 
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.LOOKING_UP);
    }

    public override void Attack() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.ATTACKING); 
    }
    
    public override void Stop() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE); 
    }


    /***** Inavailable State Change *****/
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
