using System.Collections;
using CharacterEums;
using UnityEngine;

public class DeadLowerState : PlayerLowerStateBase
{
    private const float ANIMATION_PLAYTIME = 2f;
    private Coroutine deadStateCoroutine;

    protected override void Start()
    {   
        base.Start();

        OwnerController.RegisterLowerState(PlayerLowerState.Dead, this);

        deadStateCoroutine = null;
    }

    public override PlayerLowerState GetStateType() { return PlayerLowerState.Dead; }

    public override bool DisableUpperBody() { return true; }

    public override void OnEnter()
    {
        OwnerRigid.velocity = Vector2.zero;
        deadStateCoroutine = StartCoroutine(ProcessDeadState());
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    private IEnumerator ProcessDeadState()
    {
        yield return new WaitForSeconds(ANIMATION_PLAYTIME);

        CharacterManager.Instance.ProcessPlayersDeath();
    }

    public override void Jump() { return; }
    public override void OnAir() { }
    public override void OnGround() { } 
    public override void Aim(bool isAiming) { return; }
    public override void Move(int move) { return; }
    public override void Damaged() { return; }
    public override void Tag() { return; }
    public override void Climb(bool climb) { return; }
    public override void Stop() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
