using UnityEngine;
using CharacterEums;

public interface IPlayer
{
    public void ShowCharacter(bool value);
}

public interface IPlayerLowerState
{
    public CHARACTER_LOWER_STATE GetState();
    public void StartState();
    public void UpdateState();
    public void EndState();

    public void Move(int move);
    public void Push(bool push);
    public void Stop();
    public void UpDown(int upDown);
    public void Hang(float hangingPos);
    public void Jump();
    public void Aim(bool isAiming);
    public void OnAir();
    public void OnGround();
    public void Tag();
    public void Damaged();
}

public interface IPlayerUpperState
{
    public CHARACTER_UPPER_STATE GetState();
    public void StartState();
    public void UpdateState();
    public void EndState();
    public void Move();
    public void Stop();
    public void Disable();
    public void Enable();
    public void Jump();
    public void Aim(Vector3 aim);
    public void Attack();
    public void LookUp(bool loopUp);
    public void OnAir();
    public void OnGround();
    public void Tag();
    public void Damaged();
}