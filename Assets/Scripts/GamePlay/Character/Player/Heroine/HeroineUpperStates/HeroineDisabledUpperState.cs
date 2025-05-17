using UnityEngine;

public class HeroineDisabledUpperState : HeroineUpperStateBase
{
    public override HeroineUpperState StateType => HeroineUpperState.Disabled;

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.HeroineUpper.Disabled);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroineUpperState _)
    {
    }

    public override void Enable()
    {
        StateController.ChangeState(HeroineUpperState.Idle);
    }


    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineUpper.Disabled;
}
