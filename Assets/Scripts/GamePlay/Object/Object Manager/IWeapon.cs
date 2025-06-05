using System;
using UnityEngine;

public interface IWeapon : IPoolable
{
    public WeaponType   WeaponType          { get; }
    public DamageInfo   WeaponDamageInfo    { get; }
    public bool         CanDamagePlayer     { get; }
    public float        ActiveDuration      { get; }
    public float        PostDelay           { get; }

    public event Action OnAfterWeaponUse;


    public void Attack(Vector3 vector);
    public void SetOwner(GameObject owner);
    public void SetLocalPosition(Vector3 position);

}
