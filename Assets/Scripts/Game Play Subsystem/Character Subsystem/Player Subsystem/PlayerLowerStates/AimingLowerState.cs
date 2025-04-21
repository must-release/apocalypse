using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLowerState : PlayerLowerStateBase
{
    protected override void Start()
    {
        base.Start();

        OwnerController.RegisterLowerState(PlayerLowerState.Aiming, this);
    }

    public override PlayerLowerState GetStateType() { return PlayerLowerState.Aiming; }

    public override bool DisableUpperBody() { return false; }

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
            OwnerController.ChangeLowerState(PlayerLowerState.Idle);
    }

    public override void OnAir() 
    {
        OwnerController.ChangeLowerState(PlayerLowerState.Jumping); 
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
