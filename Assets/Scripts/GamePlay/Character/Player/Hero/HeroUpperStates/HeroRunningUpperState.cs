using NUnit.Framework;
using UnityEngine;

public class HeroRunningUpperState : CommonRunningUpperState
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
}
