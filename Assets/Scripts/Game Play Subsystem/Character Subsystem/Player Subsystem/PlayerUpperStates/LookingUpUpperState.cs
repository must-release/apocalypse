using UnityEngine;
using CharacterEums;

public class LookingUpUpperState : PlayerUpperStateBase
{
    protected override void StartUpperState()
    {
        playerController.RegisterUpperState(PlayerUpperState.LookingUp, this);
    }

    public override PlayerUpperState GetStateType() { return PlayerUpperState.LookingUp; }

    public override void OnEnter()
    {
        // Rotate upper body by 90 degree
        playerController.CurrentAvatar.RotateUpperBody(90);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(PlayerUpperState nextState)
    {
        // Recover upper body rotation when not attacking
        if (PlayerUpperState.TopAttacking == nextState)
            return;
        
        playerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void LookUp(bool lookUp) 
    { 
        if( lookUp ) return;

        if ( playerController.StandingGround )
            playerController.ChangeUpperState(PlayerUpperState.Idle);
        else
            playerController.ChangeUpperState(PlayerUpperState.Jumping);
    }

    public override void Attack() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.TopAttacking); 
    }


    /***** Inavailable State Change *****/
    public override void OnGround() { return; }
    public override void Stop() { return; }
    public override void Move() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Enable() {return;}
}
