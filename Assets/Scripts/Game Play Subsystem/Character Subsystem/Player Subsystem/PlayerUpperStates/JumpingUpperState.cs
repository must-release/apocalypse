using UnityEngine;
using CharacterEums;

public class JumpingUpperState : PlayerUpperStateBase
{
    protected override void StartUpperState()
    {
        playerController.AddUpperState(PLAYER_UPPER_STATE.JUMPING, this);
    }

    public override PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.JUMPING; }

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(PLAYER_UPPER_STATE _)
    {

    }

    public override void OnGround() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE); 
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
    public override void Aim(Vector3 aim) { return; }
    public override void Stop() { return; }
    public override void Move() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Enable() {return;}
}
