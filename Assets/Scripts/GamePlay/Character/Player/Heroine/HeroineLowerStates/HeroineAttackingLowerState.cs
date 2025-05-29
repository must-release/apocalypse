using UnityEngine;

public class HeroineAttackingLowerState : HeroineLowerStateBase
{
    public override HeroineLowerState StateType => HeroineLowerState.Attacking;
    public override bool ShouldDisableUpperBody => true;

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(new Vector2(0, PlayerInfo.CurrentVelocity.y));

        StateAnimator.Play(AnimatorState.HeroineLower.Attacking);
        StateAnimator.Update(0.0f);

        _timer = 0;
        _isAttackTriggered = false;
        _attackTriggerTime = StateAnimationClip.length * _AttackTriggerRatio;
    }

    public override void OnUpdate()
    {
        _timer += Time.deltaTime;

        if (false == _isAttackTriggered && _attackTriggerTime < _timer )
        {
            _isAttackTriggered = true;
            PlayerWeapon.Attack();
        }


        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        if (1.0f <= stateInfo.normalizedTime)
        {
            var nextState = PlayerInfo.StandingGround == null ? HeroineLowerState.Jumping : HeroineLowerState.Idle;
            StateController.ChangeState(nextState);
        }
    }

    public override void OnExit(HeroineLowerState _)
    {

    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerState.Damaged);
    }


    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineLower.Attacking;


    /****** Private Members ******/

    private const float _AttackTriggerRatio = 1.0f / 3.0f;

    private float   _timer              = 0.0f;
    private float   _attackTriggerTime  = 0.0f;
    private bool    _isAttackTriggered  = false;
}
