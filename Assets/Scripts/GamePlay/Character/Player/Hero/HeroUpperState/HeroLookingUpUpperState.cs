namespace AD.GamePlay
{
    public class HeroLookingUpUpperState : CommonLookingUpUpperState
    {
        /****** Public Members ******/

        public override void LookUp(bool lookUp)
        {
            if (lookUp)
                return;

            var nextState = PlayerMovement.StandingGround == null ? HeroUpperStateType.Jumping : UpperStateType.Idle;
            StateController.ChangeState(nextState);
        }

        public override void Attack()
        {
            StateController.ChangeState(HeroUpperStateType.RunningTopAttack);
        }
    }
}
