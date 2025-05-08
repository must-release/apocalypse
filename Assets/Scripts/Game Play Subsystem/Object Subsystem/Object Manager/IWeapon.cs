using UnityEngine;

public interface IWeapon
{
    public WeaponType   WeaponType          { get; }
    public DamageInfo   WeaponDamageInfo    { get; }
    public bool         CanDamagePlayer     { get; }
    public float        ActiveDuration      { get; }
    public float        PostDelay            { get; }


    public void Attack(Vector3 vector);
    public void SetOwner(GameObject owner);
    public void SetLocalPosition(Vector3 position);

}
