using NUnit.Framework;
using UnityEngine;

public class HeroIdleLowerState : HeroLowerStateBase
{
    /****** Public Members ******/

    public override HeroLowerState StateType    => HeroLowerState.Idle;
    public override bool ShouldDisableUpperBody => false;

    public override void InitializeState(IStateController<HeroLowerState> stateController, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _IdleStateHash), "Hero animator does not have idle lower state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_IdleStateHash);
        StateAnimator.Update(0.0f);
        PlayerMotion.SetVelocity(Vector2.zero);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(HeroLowerState _)
    {

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

    public override void Move(int move)
    {
        StateController.ChangeState(HeroLowerState.Running);
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

    private readonly int _IdleStateHash = AnimatorState.Hero.GetHash(HeroLowerState.Idle);
}
