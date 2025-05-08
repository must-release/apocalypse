using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public DamageInfo WeaponDamageInfo { get; protected set; } = new DamageInfo();


    public abstract WeaponType  WeaponType          { get; }
    public abstract bool        CanDamagePlayer     { get; }
    public abstract float       ActiveDuration      { get; }
    public abstract float       PostDelay            { get; }



    public abstract void Attack(Vector3 vector);

    public void SetOwner(GameObject owner)
    {
        WeaponDamageInfo.attacker = owner;
    }

    public void SetLocalPosition(Vector3 position)
    {
        transform.localPosition = position;
    }


    /****** Protected Methods ******/

    protected virtual void Start() 
    {
        gameObject.layer = LayerMask.NameToLayer("Weapon");
    }
}
