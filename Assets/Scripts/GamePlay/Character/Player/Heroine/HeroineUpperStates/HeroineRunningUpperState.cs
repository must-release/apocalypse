using UnityEngine;

public class HeroineRunningUpperState : HeroineUpperStateBase
{
    public override HeroineUpperState StateType => HeroineUpperState.Running;

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.HeroineUpper.Running);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroineUpperState _)
    {

    }

    public override void LookUp(bool lookUp) 
    { 
        if (false == lookUp) return;

        StateController.ChangeState(HeroineUpperState.LookingUp);
    }
    
    public override void Stop() 
    {
        StateController.ChangeState(HeroineUpperState.Idle); 
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroineUpperState.Disabled);
    }

    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineUpper.Running;
}
