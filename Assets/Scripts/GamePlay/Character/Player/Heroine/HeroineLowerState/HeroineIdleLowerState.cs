using UnityEngine;

public class HeroineIdleLowerState : CommonIdleLowerState
{
    /****** Public Members ******/

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerStateType.Attacking);
    }
}
