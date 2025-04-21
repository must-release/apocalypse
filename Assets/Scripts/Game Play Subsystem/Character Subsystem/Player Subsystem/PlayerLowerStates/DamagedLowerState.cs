using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedLowerState : PlayerLowerStateBase
{
    private const float STERN_TIME = 0.4f;
    private const float KNOCK_BACK_SPEED = 15f;
    private float sternedTime;

    protected override void Start()
    {
        base.Start();

        OwnerController.RegisterLowerState(PlayerLowerState.Damaged, this);
    }

    public override PlayerLowerState GetStateType() { return PlayerLowerState.Damaged; }

    public override bool DisableUpperBody() { return true; }

    public override void OnEnter()
    {
        sternedTime = 0;

        KnockBack();
    }

    public override void OnUpdate()
    {
        sternedTime += Time.deltaTime;
        if(sternedTime > STERN_TIME)
        {
            if (OwnerController.StandingGround)
            {
                OwnerController.ChangeLowerState(PlayerLowerState.Idle);
                OwnerRigid.velocity = Vector2.zero;
            }
        }
    }

    public override void OnExit()
    {

    }

    private void KnockBack()
    {
        Vector3 attackerPos = OwnerController.RecentDamagedInfo.attacker.transform.position;
        int direction = OwnerController.transform.position.x > attackerPos.x ? 1 : -1;

        OwnerRigid.velocity = new Vector2(direction * KNOCK_BACK_SPEED, KNOCK_BACK_SPEED);
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
