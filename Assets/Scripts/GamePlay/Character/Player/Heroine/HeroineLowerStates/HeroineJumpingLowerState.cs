using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class HeroineJumpingLowerState : HeroineLowerStateBase
{
    /****** Public Members ******/

    public override HeroineLowerState   StateType               => HeroineLowerState.Jumping;
    public override bool                ShouldDisableUpperBody  => true;

    public override void InitializeState(IStateController<HeroineLowerState> stateController, 
                                          IMotionController playerPhysics, 
                                          ICharacterInfo playerInfo, 
                                          Animator stateAnimator,
                                          PlayerWeaponBase playerWeapon
    )
    {
        base.InitializeState(stateController, playerPhysics, playerInfo, stateAnimator, playerWeapon);

        Assert.IsTrue(StateAnimator.HasState(0, _JumpingStartStateHash), "Animator does not have jumping start state.");
        Assert.IsTrue(StateAnimator.HasState(0, _JumpingLoopStateHash), "Animator does not have jumping loop state.");
        Assert.IsTrue(StateAnimator.HasState(0, _JumpingEndStateHash), "Animator does not have jumping end state.");
    }

    public override void OnEnter()
    {
        if (0 < PlayerInfo.CurrentVelocity.y)
        {
            StateAnimator.Play(_JumpingStartStateHash);
            _isStartingJump = true;
            _isJumping      = true;
        }
        else
        {
            StateAnimator.Play(_JumpingLoopStateHash);
            _isStartingJump = false;
            _isJumping      = false;
        }
    }

    public override void OnUpdate()
    {
        if (0 < PlayerInfo.CurrentVelocity.y && false ==_isJumping)
                ApplyJumpCut();

        if (_isStartingJump)
            ChangeToJumpingLoopAnimation();

    }

    public override void OnExit(HeroineLowerState _)
    {
        if (null != _landCoroutine)
        {
            StopCoroutine(_landCoroutine);
            _landCoroutine = null;
        }
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
        PlayerMotion.SetVelocity(new Vector2(0, PlayerInfo.CurrentVelocity.y));
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerState.Attacking);
    }

    public override void OnGround()
    {
        _landCoroutine = StartCoroutine(LandOnGround());
    }

    public override void Climb(bool climb) 
    {
        if (false == climb) return;

        StateController.ChangeState(HeroineLowerState.Climbing);
    }

    public override void CheckJumping(bool isJumping)
    {
        if (false == _isJumping) 
            return;


        if (false == isJumping)
            _isJumping = false;
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerState.Damaged);
    }


    /****** Private Members ******/

    private readonly int _JumpingStartStateHash = AnimatorState.Heroine.GetHash(HeroineLowerState.Jumping, "Start");
    private readonly int _JumpingLoopStateHash  = AnimatorState.Heroine.GetHash(HeroineLowerState.Jumping, "Loop");
    private readonly int _JumpingEndStateHash   = AnimatorState.Heroine.GetHash(HeroineLowerState.Jumping, "End");
    private const float  _JumpFallSpeed         = 10f;

    private Coroutine _landCoroutine = null;

    private bool _isStartingJump;
    private bool _isJumping;

    private void ApplyJumpCut()
    {
        PlayerMotion.SetVelocity(new Vector2(
            PlayerInfo.CurrentVelocity.x,
            PlayerInfo.CurrentVelocity.y - _JumpFallSpeed * PlayerInfo.JumpingSpeed * Time.deltaTime
        ));
    }

    private void ChangeToJumpingLoopAnimation()
    {
        if (StateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return;

        StateAnimator.Play(_JumpingLoopStateHash);
        StateAnimator.Update(0.0f);

        _isStartingJump = false;
    }

    private IEnumerator LandOnGround()
    {
        StateAnimator.Play(_JumpingEndStateHash);
        StateAnimator.Update(0.0f);

        yield return new WaitWhile(() => StateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);

        StateController.ChangeState(HeroineLowerState.Idle);
    }
}
