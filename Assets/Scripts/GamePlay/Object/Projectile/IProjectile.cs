using System;
using UnityEngine;

public interface IProjectile : IPoolable
{
    ProjectileType  CurrentPojectileType    { get; }
    DamageInfo      ProjectileDamageInfo    { get; }
    bool            CanDamagePlayer         { get; }
    float           FireDuration            { get; }
    float           PostFireDelay           { get; }
    float           FireSpeed               { get; }
    float           GravityScale            { get; }
        
    event Action OnProjectileExpired;


    void Fire(Vector3 vector);
    void SetOwner(GameObject owner);
    void SetProjectilePosition(Vector3 position);

}
