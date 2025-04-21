using UnityEngine;
using CharacterEums;

public class JumpingUpperState : PlayerUpperStateBase
{
    public override PlayerUpperState GetStateType() { return PlayerUpperState.Jumping; }

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(PlayerUpperState _)
    {

    }

    public override void OnGround() 
    { 
        OwnerController.ChangeUpperState(PlayerUpperState.Idle); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if ( lookUp ) 
            OwnerController.ChangeUpperState(PlayerUpperState.LookingUp);
    }

    public override void Attack() 
    {   
        OwnerController.ChangeUpperState(PlayerUpperState.ATTACKING); 
    }


    /***** Inavailable State Change *****/
    public override void Aim(Vector3 aim) { return; }
    public override void Stop() { return; }
    public override void Move() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Enable() {return;}
}
