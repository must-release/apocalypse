using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HeroineAvatar : PlayerAvatarBase
{
    /****** Public Members ******/

    public override PlayerAvatarType CurrentAvatar => PlayerAvatarType.Heroine;


    /****** Protected Members ******/

    protected override void ControlLowerBody(IReadOnlyControlInfo controlInfo)
    {
        LowerState.Move(controlInfo.HorizontalInput);
        if (controlInfo.IsJumpStarted) LowerState.StartJump();
        LowerState.CheckJumping(controlInfo.IsJumping);
        if (controlInfo.IsTagging) LowerState.Tag();
        LowerState.Aim(controlInfo.AimingPosition);
        if (controlInfo.IsAttacking) LowerState.Attack();
        LowerState.UpDown(controlInfo.VerticalInput);
    }

    protected override void ControlUpperBody(IReadOnlyControlInfo controlInfo)
    {
        UpperState.Move(controlInfo.HorizontalInput);
        if (controlInfo.IsJumpStarted) UpperState.Jump();
        if (VerticalDirection.Up == controlInfo.VerticalInput) UpperState.LookUp(true);
        else UpperState.LookUp(false);
    }
}
