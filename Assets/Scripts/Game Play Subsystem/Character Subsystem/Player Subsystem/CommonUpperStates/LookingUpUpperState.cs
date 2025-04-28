using UnityEngine;
using CharacterEnums;

public class LookingUpUpperState : PlayerUpperStateBase
{

    public override CommonPlayerUpperState GetStateType() { return CommonPlayerUpperState.LookingUp; }

    public override void OnEnter()
    {
        // Rotate upper body by 90 degree
        OwnerController.CurrentAvatar.RotateUpperBody(90);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(CommonPlayerUpperState nextState)
    {
        // Recover upper body rotation when not attacking
        if (CommonPlayerUpperState.TopAttacking == nextState)
            return;
        
        OwnerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void LookUp(bool lookUp) 
    { 
        if( lookUp ) return;

        if ( OwnerController.StandingGround )
            OwnerController.ChangeUpperState(CommonPlayerUpperState.Idle);
        else
            OwnerController.ChangeUpperState(CommonPlayerUpperState.Jumping);
    }

    public override void Attack() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.TopAttacking); 
    }


    /***** Inavailable State Change *****/
    public override void OnGround() { return; }
    public override void Stop() { return; }
    public override void Move() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Enable() {return;}
}
