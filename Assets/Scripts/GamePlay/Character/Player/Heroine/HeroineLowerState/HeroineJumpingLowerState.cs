using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class HeroineJumpingLowerState : CommonJumpingLowerState
{
    /****** Public Members ******/

    public override bool ShouldDisableUpperBody => true;

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerStateType.Attacking);
    }
}
