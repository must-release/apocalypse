using NUnit.Framework;
using UnityEngine;

public class HeroLookingUpUpperState : CommonLookingUpUpperState
{
    /****** Public Members ******/

    public override void LookUp(bool lookUp)
    {
        if (lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroUpperStateType.Jumping : UpperStateType.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroUpperStateType.RunningTopAttack);
    }
}
