using CharacterEnums;
using UnityEngine;

public class DamagedLowerState : PlayerLowerStateBase<HeroineLowerState>
{
    /****** Public Members ******/

    public override HeroineLowerState   GetStateType()              => HeroineLowerState.Damaged;
    public override bool                ShouldDisableUpperBody()    => true;

    public override void OnEnter()
    {
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

    public override void OnExit()
    {

    }

    
    /****** Private Members ******/

    private const float _SternTime      = 0.4f;
    private const float _KnockBackSpeed = 15f;
    private float _sternedTime          = 0f;

    private void KnockBack()
    {
        Vector3 attackerPos = PlayerInfo.RecentDamagedInfo.attacker.transform.position;
        int direction       = PlayerInfo.CurrentPosition.x > attackerPos.x ? 1 : -1;

        PlayerMotion.SetVelocity(new Vector2(direction * _KnockBackSpeed, _KnockBackSpeed));
    }
}
