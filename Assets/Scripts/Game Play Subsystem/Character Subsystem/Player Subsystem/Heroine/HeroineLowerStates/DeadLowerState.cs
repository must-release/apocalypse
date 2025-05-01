using System.Collections;
using CharacterEnums;
using UnityEngine;

public class DeadLowerState : PlayerLowerStateBase<HeroineLowerState>
{
    /****** Public Members ******/

    public override HeroineLowerState   GetStateType()              => HeroineLowerState.Dead;
    public override bool                ShouldDisableUpperBody()    => true; 

    public override void OnEnter()
    {
        PlayerMotion.SetVelocity(Vector2.zero);
        _deadStateCoroutine = StartCoroutine(ProcessDeadState());
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    private IEnumerator ProcessDeadState()
    {
        yield return new WaitForSeconds(_AnimationPlayTime);

        CharacterManager.Instance.ProcessPlayersDeath();
    }

    /****** Private Members ******/

    private const float _AnimationPlayTime  = 2f;
    private Coroutine _deadStateCoroutine   = null;
}
