using UnityEngine;
using CharacterEums;
using System.Collections;

public interface IPlayer
{
    public bool IsLoaded {get; set;}
    public IEnumerator LoadWeaponsAndDots();
    public void ShowCharacter(bool value);
    public void RotateUpperBody(float rotateAngle);
    public void RotateUpperBody(Vector3 target);
    public void Aim(bool value);
    public float Attack(); // Execute attack and return attack cool time
}

public interface IPlayerLowerState
{
    public CHARACTER_LOWER_STATE GetState();
    public bool DisableUpperBody();
    public void StartState();
    public void UpdateState();
    public void EndState();

    public void Move(int move);
    public void Push(bool push);
    public void Stop();
    public void UpDown(int upDown);
    public void Climb(bool climb);
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
    public void Aim(Vector3 aim);
    public void Attack();
    public void LookUp(bool lookUp);
    public void Jump();
    public void OnAir();
    public void OnGround();
}