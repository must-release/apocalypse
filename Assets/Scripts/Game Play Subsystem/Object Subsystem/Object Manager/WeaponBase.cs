using System.Collections;
using UnityEngine;
using WeaponEnums;

public abstract class WeaponBase : MonoBehaviour
{
    public bool DamagePlayer {get; protected set;}
    public WEAPON_TYPE WeaponType {get; protected set;}
    public DamageInfo WeaponDamageInfo { get; protected set;}


    public abstract void Attack(Vector3 vector);

    public void SetOwner(CharacterBase owner)
    {
        if ( null == WeaponDamageInfo )
            WeaponDamageInfo = new DamageInfo();
        
        WeaponDamageInfo.attacker = owner.gameObject;
    }

    private void Awake() { InitializeWeapon(); }
    protected virtual void InitializeWeapon()
    {
        gameObject.layer = LayerMask.NameToLayer("Weapon");
        WeaponDamageInfo = new DamageInfo();
    }

    private void Update() { WeaponUpdate(); }
    protected abstract void WeaponUpdate();
}
