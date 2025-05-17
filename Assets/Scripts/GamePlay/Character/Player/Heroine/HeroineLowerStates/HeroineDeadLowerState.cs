using System.Collections;
using UnityEngine;

public class HeroineDeadLowerState : HeroineLowerStateBase
{
    /****** Public Members ******/

    public override HeroineLowerState   StateType               => HeroineLowerState.Dead;
    public override bool                ShouldDisableUpperBody  => true; 

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector2.zero);
        StateAnimator.Play(AnimatorState.HeroineLower.Dead);
        StateAnimator.Update(0.0f);

        _isAnimationPlaying = true;
    }

    public override void OnUpdate()
    {
        if (false == _isAnimationPlaying) return;

        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        if (1.0f <= stateInfo.normalizedTime)
        {
            CharacterManager.Instance.ProcessPlayersDeath();
            _isAnimationPlaying = false;
        }
    }

    public override void OnExit()
    {

    }


    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineLower.Dead;


    /****** Private Members ******/

    private bool _isAnimationPlaying = false;
}
