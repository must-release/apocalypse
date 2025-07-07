using NUnit.Framework;
using UnityEngine;

public class HeroIdleUpperState : CommonIdleUpperState
{
    /****** Public Members ******/

    public override void Attack()
    {
        StateController.ChangeState(HeroUpperStateType.Attacking);
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim) 
            return;
        
        StateController.ChangeState(HeroUpperStateType.Aiming);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroUpperStateType.Jumping);
    }
}
