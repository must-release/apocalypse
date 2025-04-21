using UnityEngine;
using CharacterEums;

public class LookingUpUpperState : PlayerUpperStateBase
{

    public override PlayerUpperState GetStateType() { return PlayerUpperState.LookingUp; }

    public override void OnEnter()
    {
        // Rotate upper body by 90 degree
        OwnerController.CurrentAvatar.RotateUpperBody(90);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(PlayerUpperState nextState)
    {
        // Recover upper body rotation when not attacking
        if (PlayerUpperState.TopAttacking == nextState)
            return;
        
        OwnerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void LookUp(bool lookUp) 
    { 
        if( lookUp ) return;

        if ( OwnerController.StandingGround )
            OwnerController.ChangeUpperState(PlayerUpperState.Idle);
        else
            OwnerController.ChangeUpperState(PlayerUpperState.Jumping);
    }

    public override void Attack() 
    { 
        OwnerController.ChangeUpperState(PlayerUpperState.TopAttacking); 
    }


    /***** Inavailable State Change *****/
    public override void OnGround() { return; }
    public override void Stop() { return; }
    public override void Move() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Enable() {return;}
}
