using UnityEngine;

namespace AD.GamePlay
{
    public class HeroineRunningLowerState : CommonRunningLowerState
    {
        /****** Public Members ******/

        public override void Attack()
        {
            StateController.ChangeState(HeroineLowerStateType.Attacking);
        }
    }
}
