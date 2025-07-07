using UnityEngine;

public class HeroineRunningLowerState : CommonRunningLowerState
{
    /****** Public Members ******/

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerStateType.Attacking);
    }
}
