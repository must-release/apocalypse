using UnityEngine;

public class CommonTaggingLowerState : PlayerLowerState
{
    /****** Public Members ******/

    public override LowerStateType CurrentState => LowerStateType.Tagging;
    public override bool ShouldDisableUpperBody => true;

    public override void OnEnter()
    {
        // Stop player
        PlayerMotion.SetVelocity(Vector2.up * _popping);

        // Reset Time
        _time = 0f;
    }

    public override void OnUpdate()
    {
        _time += Time.deltaTime;

        if (_time < _taggingTime) return;

        var nextState = _isOnAir ? LowerStateType.Jumping : LowerStateType.Idle;
        StateController.ChangeState(nextState);
    }

    public override void OnAir() { _isOnAir = true; }

    public override void OnGround() { _isOnAir = false; }


    /****** Private Members ******/

    private float _taggingTime = 0.2f;
    private float _time = 0f;
    private float _popping = 3f;
    private bool _isOnAir = false;

}
