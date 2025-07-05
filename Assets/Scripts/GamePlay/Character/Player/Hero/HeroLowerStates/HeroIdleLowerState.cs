using NUnit.Framework;
using UnityEngine;

public class HeroIdleLowerState : HeroLowerStateBase
{
    /****** Public Members ******/

    public override HeroLowerStateType StateType    => HeroLowerStateType.Idle;
    public override bool ShouldDisableUpperBody => false;

    public override void InitializeState(IStateController<HeroLowerStateType> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);
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

    public override void OnExit(HeroLowerStateType _)
    {

    }

    public override void StartJump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed));
        StateController.ChangeState(HeroLowerStateType.Jumping);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroLowerStateType.Jumping);
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim) return;

        StateController.ChangeState(HeroLowerStateType.Aiming);
    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        if (HorizontalDirection.None != horizontalInput)
        {
            StateController.ChangeState(HeroLowerStateType.Running);
        }
    }

    public override void Tag()
    {
        StateController.ChangeState(HeroLowerStateType.Tagging);
    }

    public override void Climb(bool climb)
    {
        if (false == climb) return;

        StateController.ChangeState(HeroLowerStateType.Climbing);
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroLowerStateType.Damaged);
    }


    /****** Private Members ******/

    private readonly int _IdleStateHash = AnimatorState.Hero.GetHash(HeroLowerStateType.Idle);
}
