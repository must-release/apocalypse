using UnityEngine;

public class HeroineIdleLowerState : HeroineLowerState
{

    public override HeroineLowerStateType   StateType               => HeroineLowerStateType.Idle;
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

    public override void OnExit(HeroineLowerStateType _)
    {

    }

    public override void StartJump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed));
        StateController.ChangeState(HeroineLowerStateType.Jumping);
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerStateType.Attacking);
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroineLowerStateType.Jumping);
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim) return;

        StateController.ChangeState(HeroineLowerStateType.Aiming);
    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        if (HorizontalDirection.None == horizontalInput) 
            return;

        StateController.ChangeState(HeroineLowerStateType.Running);
    }

    public override void Tag()
    {
        StateController.ChangeState(HeroineLowerStateType.Tagging);
    }

    public override void UpDown(VerticalDirection verticalInput)
    {
        if (null == ObjectInteractor.CurrentClimbableObject || VerticalDirection.None == verticalInput ) return;

        var refPos = ObjectInteractor.CurrentClimbableObject.GetClimbReferencePoint();
        var curPos = PlayerInfo.CurrentPosition;

        if ((curPos.y < refPos.y && VerticalDirection.Up == verticalInput) || (refPos.y < curPos.y && VerticalDirection.Down == verticalInput))
        {
            StateController.ChangeState(HeroineLowerStateType.Climbing);
        }
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerStateType.Damaged);
    }
}
