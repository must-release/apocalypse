using UnityEngine;

public class HeroTaggingLowerState : HeroLowerStateBase
{
    /****** Public Members ******/

    public override HeroLowerState StateType    => HeroLowerState.Tagging;
    public override bool ShouldDisableUpperBody => true;

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector2.up * _popping);

        _time = 0f;
    }

    public override void OnUpdate()
    {
        _time += Time.deltaTime;

        if (_time < _taggingTime) return;

        var nextState = _isOnAir ? HeroLowerState.Jumping : HeroLowerState.Idle;
        StateController.ChangeState(nextState);
    }

    public override void OnExit(HeroLowerState _)
    {

    }

    public override void OnAir() { _isOnAir = true; }

    public override void OnGround() { _isOnAir = false; }


    /****** Private Members ******/

    private float _taggingTime = 0.2f;
    private float _time = 0f;
    private float _popping = 3f;
    private bool _isOnAir = false;

}
