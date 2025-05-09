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


    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineLower.Dead;


    /****** Private Members ******/

    private const float _AnimationPlayTime  = 2f;
    private Coroutine _deadStateCoroutine   = null;
}
