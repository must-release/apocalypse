using UnityEngine;

public class HeroineIdleLowerState : HeroineLowerStateBase
{

    public override HeroineLowerState   StateType               => HeroineLowerState.Idle;
    public override bool                ShouldDisableUpperBody  => false; 

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.Heroine.GetHash(StateType));
        StateAnimator.Update(0.0f);
        PlayerMotion.SetVelocity(Vector2.zero);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(HeroineLowerState _)
    {

    }

    public override void StartJump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed));
        StateController.ChangeState(HeroineLowerState.Jumping);
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerState.Attacking);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroineLowerState.Jumping);
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim) return;

        StateController.ChangeState(HeroineLowerState.Aiming);
    }

    public override void Move(int move)
    {
        StateController.ChangeState(HeroineLowerState.Running);
    }

    public override void Tag()
    {
        StateController.ChangeState(HeroineLowerState.Tagging);
    }

    public override void Climb(bool climb)
    {
        if (false == climb) return;

        StateController.ChangeState(HeroineLowerState.Climbing);
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerState.Damaged);
    }
}
