using UnityEngine;
using CharacterEums;

public class LookingUpUpperState : PlayerUpperStateBase
{
    protected override void StartUpperState()
    {
        playerController.AddUpperState(PLAYER_UPPER_STATE.LOOKING_UP, this);
    }

    public override PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.LOOKING_UP; }

    public override void OnEnter()
    {
        // Rotate upper body by 90 degree
        playerController.CurrentPlayer.RotateUpperBody(90);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(PLAYER_UPPER_STATE nextState)
    {
        // Recover upper body rotation when not attacking
        if (PLAYER_UPPER_STATE.TOP_ATTACKING == nextState)
            return;
        
        playerController.CurrentPlayer.RotateUpperBody(0);
    }

    public override void LookUp(bool lookUp) 
    { 
        if( lookUp ) return;

        if ( playerController.StandingGround )
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE);
        else
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING);
    }

    public override void Attack() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.TOP_ATTACKING); 
    }


    /***** Inavailable State Change *****/
    public override void OnGround() { return; }
    public override void Stop() { return; }
    public override void Move() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Enable() {return;}
}
