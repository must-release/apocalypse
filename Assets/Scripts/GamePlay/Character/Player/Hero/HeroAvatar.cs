namespace AD.GamePlay
{
    public class HeroAvatar : PlayerAvatarBase
    {
        /****** Public Members ******/

        public override PlayerAvatarType CurrentAvatar => PlayerAvatarType.Hero;


        /****** Protected Members ******/

        protected override void ControlLowerBody(IReadOnlyControlInfo controlInfo)
        {
            LowerState.Move(controlInfo.HorizontalInput);
            if (controlInfo.IsJumpStarted) LowerState.StartJump();
            LowerState.CheckJumping(controlInfo.IsJumping);
            if (controlInfo.IsAttacking) LowerState.Attack();
            if (controlInfo.IsTagging) LowerState.Tag();
            LowerState.Aim(controlInfo.AimingPosition);
            LowerState.UpDown(controlInfo.VerticalInput);
        }

        protected override void ControlUpperBody(IReadOnlyControlInfo controlInfo)
        {
            UpperState.Move(controlInfo.HorizontalInput);
            if (controlInfo.IsJumpStarted) UpperState.Jump();
            UpperState.Aim(controlInfo.AimingPosition);
            if (VerticalDirection.Up == controlInfo.VerticalInput) UpperState.LookUp(true);
            else UpperState.LookUp(false);
            if (controlInfo.IsAttacking) UpperState.Attack();
        }
    }
}