using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HeroineAvatar : PlayerAvatarBase
{
    /****** Public Members ******/

    public override PlayerType CurrentPlayer => PlayerType.Heroine;


    /****** Protected Members ******/

    protected override IPlayerLowerState LowerState
    {
        get => _lowerState;
        set
        {
            _lowerState = value as HeroineLowerState;
            Assert.IsTrue(null != _lowerState, $"{value.StateType} is not a HeroineLowerState.");
        }
    }

    protected override IPlayerUpperState UpperState
    {
        get => _upperState;
        set
        {
            _upperState = value as IHeroineUpperState;
            Assert.IsTrue(null != _upperState, $"{value.StateType} is not a HeroineUpperState.");
        }
    }

    protected override void ControlLowerBody(IReadOnlyControlInfo controlInfo)
    {
        _lowerState.Move(controlInfo.HorizontalInput);
        if (controlInfo.IsJumpStarted) _lowerState.StartJump();
        _lowerState.CheckJumping(controlInfo.IsJumping);
        if (controlInfo.IsTagging) _lowerState.Tag();
        _lowerState.Aim(controlInfo.AimingPosition);
        if (controlInfo.IsAttacking) _lowerState.Attack();
        _lowerState.UpDown(controlInfo.VerticalInput);
    }

    protected override void ControlUpperBody(IReadOnlyControlInfo controlInfo)
    {
        _upperState.Move(controlInfo.HorizontalInput);
        if (controlInfo.IsJumpStarted) _upperState.Jump();
        _upperState.Aim(controlInfo.AimingPosition);
        if (VerticalDirection.Up == controlInfo.VerticalInput) _upperState.LookUp(true);
        else _upperState.LookUp(false);
    }


    /****** Private Members ******/

    private IHeroineUpperState _upperState;
    private HeroineLowerState _lowerState;
}
