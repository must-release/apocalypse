using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLowerState : PlayerLowerStateBase
{
    public override CommonPlayerLowerState GetStateType() { return CommonPlayerLowerState.Aiming; }

    public override bool ShouldDisableUpperBody() { return false; }

    public override void OnEnter()
    {
        OwnerRigid.velocity = Vector2.zero;
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    public override void Aim(bool isAiming) 
    { 
        if(!isAiming) 
            OwnerController.ChangeLowerState(CommonPlayerLowerState.Idle);
    }

    public override void OnAir() 
    {
        OwnerController.ChangeLowerState(CommonPlayerLowerState.Jumping); 
    }
    
    public override void Move(int move) { return; }
    public override void Climb(bool climb) { return; }
    public override void Jump() {return;}
    public override void Tag() { return; }
    public override void OnGround() { return; }
    public override void Stop() { return; }
    public override void Push(bool push) { return; }
    public override void UpDown(int upDown) { return; }
}
