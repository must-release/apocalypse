using System.Collections;
using UnityEngine;

public class HeroineDeadLowerState : HeroineLowerState
{
    /****** Public Members ******/

    public override HeroineLowerStateType   StateType               => HeroineLowerStateType.Dead;
    public override bool                ShouldDisableUpperBody  => true; 

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector2.zero);
        StateAnimator.Play(AnimatorState.Heroine.GetHash(StateType));
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

    public override void OnExit(HeroineLowerStateType _)
    {

    }


    /****** Private Members ******/

    private bool _isAnimationPlaying = false;
}
