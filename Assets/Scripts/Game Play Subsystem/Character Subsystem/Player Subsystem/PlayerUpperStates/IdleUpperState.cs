using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleUpperState : PlayerUpperStateBase
{
    protected override void StartUpperState()
    {
        playerController.AddUpperState(PLAYER_UPPER_STATE.IDLE, this);
    }

    public override PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.IDLE; }

    public override void OnEnter()
    {
        playerController.UpperAnimator.SetBool("Move", false);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(PLAYER_UPPER_STATE _)
    {

    }

    public override void Move() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.RUNNING); 
    }

    public override void OnAir() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if ( lookUp ) 
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.LOOKING_UP);
    }

    public override void Attack() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.ATTACKING); 
    }


    /***** Inavailable State Change *****/
    public override void Jump() { return; }
    public override void Stop() { return; }
    public override void OnGround() { return; }
    public override void Enable() { return; }
}
