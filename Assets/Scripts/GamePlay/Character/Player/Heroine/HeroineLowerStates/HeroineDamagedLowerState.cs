using UnityEngine;

public class HeroineDamagedLowerState : HeroineLowerStateBase
{
    /****** Public Members ******/

    public override HeroineLowerState   StateType               => HeroineLowerState.Damaged;
    public override bool                ShouldDisableUpperBody  => true;

    public override void OnEnter()
    {
        StateAnimator.Play(_DamagedStateHash);

        _sternedTime = 0f;

        KnockBack();
    }

    public override void OnUpdate()
    {
        _sternedTime += Time.deltaTime;

        if (_sternedTime < _SternTime) 
            return;

        if (null == PlayerInfo.StandingGround)
            return;

        StateController.ChangeState(HeroineLowerState.Idle);
        PlayerMotion.SetVelocity(Vector2.zero);
    }

    public override void OnExit(HeroineLowerState _)
    {

    }


    /****** Private Members ******/

    private readonly int _DamagedStateHash = AnimatorState.Heroine.GetHash(HeroineLowerState.Damaged);

    private const float _SternTime      = 0.4f;
    private const float _KnockBackSpeed = 6f;

    private float _sternedTime = 0f;

    private void KnockBack()
    {
        Vector3 attackerPos = PlayerInfo.RecentDamagedInfo.Attacker.transform.position;
        int direction       = PlayerInfo.CurrentPosition.x > attackerPos.x ? 1 : -1;

        PlayerMotion.SetVelocity(new Vector2(direction * _KnockBackSpeed, _KnockBackSpeed));
    }
}
