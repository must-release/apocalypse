using UnityEngine;

public class HeroIdleLowerState : CommonIdleLowerState
{
    /****** Public Members ******/

    public override void Attack()
    {
        StateController.ChangeState(HeroLowerStateType.StandingAttack);
    }

    public override void UpDown(VerticalDirection verticalInput)
    {
        base.UpDown(verticalInput);

        if (VerticalDirection.Up == verticalInput)
        {
            StateController.ChangeState(HeroLowerStateType.IdleLookingUp);
        }
    }
}