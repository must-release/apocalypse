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
        // Do not call base.UpDown function

        if (null != ObjectInteractor.CurrentClimbableObject && VerticalDirection.None != verticalInput)
        {
            var refPos = ObjectInteractor.CurrentClimbableObject.GetClimbReferencePoint();
            var curPos = PlayerInfo.CurrentPosition;

            if ((curPos.y < refPos.y && VerticalDirection.Up == verticalInput) || (refPos.y < curPos.y && VerticalDirection.Down == verticalInput))
            {
                StateController.ChangeState(LowerStateType.Climbing);
            }   
        }
        else if (VerticalDirection.Up == verticalInput)
        {
            StateController.ChangeState(HeroLowerStateType.IdleLookingUp);
        }
    }
}