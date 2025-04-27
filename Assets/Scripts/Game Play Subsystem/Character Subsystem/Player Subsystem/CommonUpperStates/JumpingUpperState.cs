using UnityEngine;
using CharacterEums;

public class JumpingUpperState : PlayerUpperStateBase
{
    public override CommonPlayerUpperState GetStateType() { return CommonPlayerUpperState.Jumping; }

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(CommonPlayerUpperState _)
    {

    }

    public override void OnGround() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Idle); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if ( lookUp ) 
            OwnerController.ChangeUpperState(CommonPlayerUpperState.LookingUp);
    }

    public override void Attack() 
    {   
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Attacking); 
    }


    /***** Inavailable State Change *****/
    public override void Aim(Vector3 aim) { return; }
    public override void Stop() { return; }
    public override void Move() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Enable() {return;}
}
