using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public DamageInfo WeaponDamageInfo { get; protected set; } = new DamageInfo();


    public abstract WeaponType  WeaponType          { get; }
    public abstract bool        CanDamagePlayer     { get; }
    public abstract float       ActiveDuration      { get; }
    public abstract float       PostDelay           { get; }



    public abstract void Attack(Vector3 vector);

    public virtual void SetOwner(GameObject owner)
    {
        Assert.IsTrue(null != owner, "Owner of the weapon is not assigned");

        WeaponDamageInfo.attacker = owner;
    }

    public void SetLocalPosition(Vector3 position)
    {
        transform.localPosition = position;
    }


    /****** Protected Methods ******/

    protected virtual void Awake() 
    {
        gameObject.layer    = LayerMask.NameToLayer("Weapon");
    }
}
