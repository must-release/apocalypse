﻿using NUnit.Framework;
using UnityEngine;

public class HeroRunningLowerState : HeroLowerStateBase
{
    /****** Public Members ******/

    public override HeroLowerState StateType    => HeroLowerState.Running;
    public override bool ShouldDisableUpperBody => false;

    public override void InitializeState(IStateController<HeroLowerState> stateController, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _RunningStateHash), "Hero animator does not have running lower state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_RunningStateHash);
        StateAnimator.Update(0.0f);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(HeroLowerState _)
    {
    }

    public override void Move(int move)
    {
        if (0 == move) return;

        // move player
        PlayerMotion.SetVelocity(new Vector2(move * PlayerInfo.MovingSpeed, PlayerInfo.CurrentVelocity.y));

        // set direction
        FacingDirection direction = move < 0 ? FacingDirection.Left : FacingDirection.Right;
        if (direction != PlayerInfo.CurrentFacingDirection)
        {
            PlayerMotion.SetFacingDirection(direction);
        }
    }

    public override void Stop()
    {
        StateController.ChangeState(HeroLowerState.Idle);
    }

    public override void StartJump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed));
        StateController.ChangeState(HeroLowerState.Jumping);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroLowerState.Jumping);
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim) return;

        StateController.ChangeState(HeroLowerState.Aiming);
    }

    public override void Tag()
    {
        StateController.ChangeState(HeroLowerState.Tagging);
    }

    public override void Climb(bool climb)
    {
        if (false == climb) return;

        StateController.ChangeState(HeroLowerState.Climbing);
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroLowerState.Damaged);
    }


    /****** Private Members ******/

    private readonly int _RunningStateHash = AnimatorState.Hero.GetHash(HeroLowerState.Running);
}
